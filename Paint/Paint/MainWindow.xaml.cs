using IContract;
using Paint.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        // State
        bool _isDrawing = false;
        bool _isSaved = false;
        string _currentType = "";
        IShapeEntity _preview;
        Point _start;
        List<IShapeEntity> _drawnShapes = new List<IShapeEntity>();
        List<IShapeEntity> allShape = new List<IShapeEntity>();
        private Stack<IShapeEntity> _buffer = new Stack<IShapeEntity>();
        string _backgroundImagePath = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Config.firstConfig();

            Title = $"Tìm thấy {Config.shapesPrototypes.Count} hình";

            // Tạo ra các nút bấm tương ứng
            
            foreach (IShapeEntity shape in Config.shapesPrototypes.Values)
            {
                allShape.Add(shape);
            }
            iconListView.ItemsSource = allShape;


            //foreach (var (name, entity) in Config.shapesPrototypes)
            //{
            //    var button = new Button();
            //    button.Content = name;
            //    button.Tag = entity;
            //    button.Width = 80;
            //    button.Height = 35;
            //    button.Click += Button_Click;

            //    //TODO: thêm các nút bấm vào giao diện
            //    actionsStackPanel.Children.Add(button);
            //}

            if (Config.shapesPrototypes.Count > 0)
            {
                //Lựa chọn nút bấm đầu tiên
                var (key, shape) = Config.shapesPrototypes.ElementAt(0);
                _currentType = key;
                _preview = (shape.Clone() as IShapeEntity)!;
            }

        }

        // Đổi lựa chọn
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var entity = button!.Tag as IShapeEntity;

            _currentType = entity!.Name;
            _preview = (Config.shapesPrototypes[entity.Name].Clone() as IShapeEntity)!;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
            _start = e.GetPosition(canvas);

            _preview.HandleStart(_start);
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                var end = e.GetPosition(canvas);
                _preview.HandleEnd(end);

                // Xóa đi tất cả bản vẽ cũ và vẽ lại những đường thẳng trước đó
                canvas.Children.Clear(); // Xóa đi toàn bộ

                // Vẽ lại những hình đã vẽ trước đó
                foreach (var item in _drawnShapes)
                {
                    var painter = Config.painterPrototypes[item.Name];
                    var shape = painter.Draw(item);

                    canvas.Children.Add(shape);
                }

                var previewPainter = Config.painterPrototypes[_preview.Name];
                var previewElement = previewPainter.Draw(_preview);
                canvas.Children.Add(previewElement);
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;

            var end = e.GetPosition(canvas); // Điểm kết thúc

            _preview.HandleEnd(end);

            _drawnShapes.Add((IShapeEntity)_preview.Clone());
        }

        private void createNewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_backgroundImagePath.Length > 0 && _drawnShapes.Count == 0)
            {
                _backgroundImagePath = "";
                canvas.Background = new SolidColorBrush(Colors.White);
            }
            if (_drawnShapes.Count == 0)
            {
                return;
            }

            if (_isSaved)
            {
                ResetToDefault();
                return;
            }

            var result = MessageBox.Show("Would you like to save current file?", "Unsaved changes detected", MessageBoxButton.YesNoCancel);

            if (MessageBoxResult.Yes == result)
            {
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                var serializedShapeList = JsonConvert.SerializeObject(_drawnShapes, settings);

                StringBuilder builder = new StringBuilder();
                builder.Append(serializedShapeList).Append("\n").Append($"{_backgroundImagePath}");
                string content = builder.ToString();

                byte[] bytes = Encoding.ASCII.GetBytes(content);
                string data = Convert.ToBase64String(bytes);

                var dialog = new System.Windows.Forms.SaveFileDialog();

                //dialog.Filter = "JSON (*.json)|*.json";
                dialog.Filter = "DAT File (.dat)|*.dat";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;
                    //File.WriteAllText(path, content);
                    File.WriteAllText(path, data);
                }

                ResetToDefault();
                _isSaved = true;
            }
            else if (MessageBoxResult.No == result)
            {
                ResetToDefault();
                return;
            }
            else if (MessageBoxResult.Cancel == result)
            {
                return;
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            // New Official
            var dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.Filter = "DAT File (.dat)|*.dat";

            var containers = new List<IShapeEntity>();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _drawnShapes.Clear();
                canvas.Children.Clear();

                string path = dialog.FileName;

                string[] src = File.ReadAllLines(path);
                string content = "";
                foreach (string line in src)
                    content += line;

                _drawnShapes.Clear();
                canvas.Children.Clear();

                string[] shapes = content.Split('[', ']', StringSplitOptions.RemoveEmptyEntries);
                foreach (string shape in shapes)
                {
                    string[] properties = shape.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var property in properties)
                    {
                        string[] pairs = property.Split('#', StringSplitOptions.RemoveEmptyEntries);
                        dict.Add(pairs[0].Trim(), pairs[1].Trim());
                    }

                    var start = dict.ElementAt(0);
                    string[] startCoords = start.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Point startPoint = new Point(double.Parse(startCoords[0]), double.Parse(startCoords[1]));

                    var end = dict.ElementAt(1);
                    string[] endCoords = end.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Point endPoint = new Point(double.Parse(endCoords[0]), double.Parse(endCoords[1]));

                    IShapeEntity shapeEntity = (Config.shapesPrototypes[dict["Name"]].Clone() as IShapeEntity)!;
                    shapeEntity.HandleStart(startPoint);
                    shapeEntity.HandleEnd(endPoint);
                   
                    containers.Add(shapeEntity);
                }
            }

            foreach (var item in containers)
                _drawnShapes.Add(item);

            foreach (var shape in _drawnShapes)
            {
                var painter = Config.painterPrototypes[shape.Name];
                var item = painter.Draw(shape);
                canvas.Children.Add(item);
            }

            // Official
            //var dialog = new System.Windows.Forms.OpenFileDialog();

            //dialog.Filter = "DAT File (.dat)|*.dat";

            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string path = dialog.FileName;

            //    string[] content = File.ReadAllLines(path);

            //    string background = "";
            //    string json = content[0];
            //    if (content.Length > 1)
            //        background = content[1];

            //    byte[] textAsBytes = System.Convert.FromBase64String(json);
            //    var src = Encoding.ASCII.GetString(textAsBytes);

            //    var settings = new JsonSerializerSettings()
            //    {
            //        TypeNameHandling = TypeNameHandling.Objects
            //    };

            //    _drawnShapes.Clear();
            //    _backgroundImagePath = background;
            //    canvas.Children.Clear();

            //    List<IShapeEntity> containers = JsonConvert.DeserializeObject<List<IShapeEntity>>(src, settings);

            //    foreach (var item in containers)
            //        _drawnShapes.Add(item);

            //    if (_backgroundImagePath.Length != 0)
            //    {
            //        ImageBrush brush = new ImageBrush();
            //        brush.ImageSource = new BitmapImage(new Uri(_backgroundImagePath, UriKind.Absolute));
            //        canvas.Background = brush;
            //    }
            //}

            //foreach (var shape in _drawnShapes)
            //{
            //    var painter = Config.painterPrototypes[shape.Name];
            //    var item = painter.Draw(shape);
            //    canvas.Children.Add(item);
            //}
        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // New official
            var dialog = new System.Windows.Forms.SaveFileDialog();

            dialog.Filter = "DAT File (.dat)|*.dat";

            string content = "";

            for (int index = 0; index < _drawnShapes.Count; index++)
            {
                var dict = DictionaryFromType(_drawnShapes.ElementAt(index));

                content += "[";
                for (int count = 0; count < dict.Count; count++)
                {
                    var element = dict.ElementAt(count);
                    var Key = element.Key;
                    var Value = element.Value;
                    content += Key + "#" + Value;

                    System.Diagnostics.Debug.WriteLine(Key + " " + Value);

                    if (count != dict.Count - 1)
                        content += ";";
                }
                content += "]";
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                File.WriteAllText(path, content);
                _isSaved = true;
            }


            // Official
            //var settings = new JsonSerializerSettings()
            //{
            //   TypeNameHandling = TypeNameHandling.Objects
            //};

            //var serializedShapeList = JsonConvert.SerializeObject(_drawnShapes, settings);

            //StringBuilder builder = new StringBuilder();
            //builder.Append(serializedShapeList).Append("\n").Append($"{_backgroundImagePath}");
            //string content = builder.ToString();

            //byte[] bytes = Encoding.ASCII.GetBytes(content);
            //string data = Convert.ToBase64String(bytes);

            //var dialog = new System.Windows.Forms.SaveFileDialog();

            //dialog.Filter = "DAT File (.dat)|*.dat";

            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string path = dialog.FileName;
            //    File.WriteAllText(path, data);
            //    _isSaved = true;
            //}
        }

        public Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null) return new Dictionary<string, object>();
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }
            return dict;
        }

        private void SaveCanvasToImage(Canvas canvas, string filename, string extension = "png")
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            switch (extension)
            {
                case "png":
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        pngEncoder.Save(file);
                    }
                    break;
                case "jpeg":
                    JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        jpegEncoder.Save(file);
                    }
                    break;
                case "bmp":

                    BmpBitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                    bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream file = File.Create(filename))
                    {
                        bitmapEncoder.Save(file);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ResetToDefault()
        {
            if (this.allShape.Count == 0)
                return;

            _isSaved = false;
            _isDrawing = false;

            _drawnShapes.Clear();

            _backgroundImagePath = "";

            dashComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndex = 0;

            EditMode.Header = "Draw Mode";
            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(Colors.White);
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                _backgroundImagePath = path;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
                canvas.Background = brush;
            }
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                string extension = path.Substring(path.LastIndexOf('\\') + 1).Split('.')[1];

                SaveCanvasToImage(canvas, path, extension);
            }
            _isSaved = true;
        }

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_drawnShapes.Count == 0)
                return;
            if (_drawnShapes.Count == 0 && _buffer.Count == 0)
                return;

            // Push last shape into buffer and remove it from final list, then re-draw canvas
            int lastIndex = _drawnShapes.Count - 1;
            _buffer.Push(_drawnShapes[lastIndex]);
            _drawnShapes.RemoveAt(lastIndex);

            RedrawCanvas();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_buffer.Count == 0)
                return;
            if (_drawnShapes.Count == 0 && _buffer.Count == 0)
                return;

            _drawnShapes.Add(_buffer.Pop());
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            canvas.Children.Clear();
            Console.WriteLine(_drawnShapes.Count);
            foreach (var shape in (_drawnShapes))
            {
                var painter = Config.painterPrototypes[shape.Name];
                var item = painter.Draw(shape);
                canvas.Children.Add(item);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.allShape.Count == 0)
                return;

            var index = iconListView.SelectedIndex;

            _currentType = allShape[index].Name;

            _preview = (Config.shapesPrototypes[allShape[index].Name].Clone() as IShapeEntity)!;
        }

        private void dashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnBasicBlack_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicOrange_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicYellow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicBlue_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicGreen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicPurple_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicPink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicBrown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editColorButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicGray_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBasicRed_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ExportToPng(String path, Canvas surface1)
        {
            if (path == null) return;

            Canvas surface = surface1;
            surface.Background = new SolidColorBrush(Colors.White);

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;

            
        }
    }
}
