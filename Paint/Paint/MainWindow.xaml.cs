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
using System.Diagnostics;
using Paint.AdvancedFeature;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        // State
        bool _isDrawing = false;
        bool _isSaved = false;
        string _currentType = "";
        public IShapeEntity _preview;
        Point _start;
        public Point _newStartPoint;
        public List<IShapeEntity> _drawnShapes = new List<IShapeEntity>();
        List<IShapeEntity> allShape = new List<IShapeEntity>();
        string _backgroundImagePath = "";
        public IShapeEntity? _choosenShape = null;
        public IShapeEntity? _clipboard = null;
        public List<Image> _images = new List<Image>();
        public Point tmp_position = new Point();
        

        public float zoomRatio { get; set; } = ZoomCommand.DEFAULT_ZOOM_VALUE;

        private static int _currentThickness = 1;
        private static SolidColorBrush _currentColor = new SolidColorBrush(Colors.Red);
        private static DoubleCollection _currentDash = null;

        bool _isFilling = false;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
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

            if (Config.shapesPrototypes.Count > 0)
            {
                //Lựa chọn nút bấm đầu tiên
                var (key, shape) = Config.shapesPrototypes.ElementAt(0);
                _currentType = key;
                _preview = (shape.Clone() as IShapeEntity)!;
            }
            redoButton.IsEnabled = undoButton.IsEnabled = false;
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
            if(e.ChangedButton == MouseButton.Left)
            {
                _isDrawing = true;
                _start = e.GetPosition(canvas);

                _preview.HandleStart(_start);
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            tmp_position = e.GetPosition(canvas);

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

                foreach(var item in _images)
                {
                    canvas.Children.Add(item);
                }

                _preview.HandleSolidColorBrush(_currentColor);
                _preview.HandleThickness(_currentThickness);
                _preview.HandleDoubleCollection(_currentDash);

                var previewPainter = Config.painterPrototypes[_preview.Name];
                var previewElement = previewPainter.Draw(_preview);
                canvas.Children.Add(previewElement);
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                _isDrawing = false;

                var end = e.GetPosition(canvas); // Điểm kết thúc

                _preview.HandleEnd(end);

                if(Math.Abs(_start.X - end.X) < 2 && Math.Abs(_start.Y - end.Y) < 2)
                {
                    Debug.WriteLine("Small");
                    foreach(var item in _drawnShapes)
                    {
                        if(item.isHovering(_start.X, _start.Y))
                        {
                            _choosenShape = item;

                            if (_isFilling == true)
                            {
                               _choosenShape.HandleBackground(_currentColor);

                                RedrawCanvas();

                                _isFilling = false;
                            }
                        }
                    }
                }
                else
                {
                    Command.executeCommand(new DrawCommand(this));
                    undoButton.IsEnabled = true;
                    redoButton.IsEnabled = false;
                }
            }
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

            var result = MessageBox.Show("Save your changes to this file?", "Unsaved changes detected", MessageBoxButton.YesNoCancel);

            if (MessageBoxResult.Yes == result)
            {
                //var settings = new JsonSerializerSettings()
                //{
                //    TypeNameHandling = TypeNameHandling.Objects
                //};

                //var serializedShapeList = JsonConvert.SerializeObject(_drawnShapes, settings);

                //StringBuilder builder = new StringBuilder();
                //builder.Append(serializedShapeList).Append("\n").Append($"{_backgroundImagePath}");
                //string content = builder.ToString();

                //byte[] bytes = Encoding.ASCII.GetBytes(content);
                //string data = Convert.ToBase64String(bytes);

                //var dialog = new System.Windows.Forms.SaveFileDialog();

                ////dialog.Filter = "JSON (*.json)|*.json";
                //dialog.Filter = "DAT File (.dat)|*.dat";

                //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    string path = dialog.FileName;
                //    //File.WriteAllText(path, content);
                //    File.WriteAllText(path, data);
                //}

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
                        content += Key + "@" + Value;

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

                string[] shapes = content.Split("[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string shape in shapes)
                {
                    string[] properties = shape.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var property in properties)
                    {
                        string[] pairs = property.Split('@', StringSplitOptions.RemoveEmptyEntries);
                        dict.Add(pairs[0].Trim(), pairs[1].Trim());
                    }

                    var start = dict.ElementAt(0);
                    string[] startCoords = start.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Point startPoint = new Point(double.Parse(startCoords[0]), double.Parse(startCoords[1]));

                    var end = dict.ElementAt(1);
                    string[] endCoords = end.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Point endPoint = new Point(double.Parse(endCoords[0]), double.Parse(endCoords[1]));

                    var color = (Color)ColorConverter.ConvertFromString(dict["Brush"]);
                    var brush = new SolidColorBrush(color);

                    IShapeEntity shapeEntity = (Config.shapesPrototypes[dict["Name"]].Clone() as IShapeEntity)!;
                    shapeEntity.HandleStart(startPoint);
                    shapeEntity.HandleEnd(endPoint);
                    shapeEntity.HandleSolidColorBrush(brush);
                    shapeEntity.HandleThickness(int.Parse(dict["Thickness"]));

                    var dash = dict["StrokeDash"];
                    if (dash.CompareTo("null") == 0)
                    {
                        shapeEntity.HandleDoubleCollection(null);
                    }
                    else
                    {
                        shapeEntity.HandleDoubleCollection(DoubleCollection.Parse(dash));
                    }

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
                    content += Key + "@" + Value;

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

                if (prp.Name.CompareTo("StrokeDash") == 0)
                {
                    if (value == null)
                    {
                        value = "null";
                    }
                }
                dict.Add(prp.Name, value);
            }
            return dict;
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

                FileHandle.SaveCanvasToImage(canvas, path, extension);
            }
            _isSaved = true;
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new UndoCommand(this));
            RedrawCanvas();
        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new RedoCommand(this));
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            canvas.Children.Clear();
            Console.WriteLine(_drawnShapes.Count);
            
            redoButton.IsEnabled = Command._redoHistory.count() > 0;
            undoButton.IsEnabled = Command._undoHistory.count() > 0 && _drawnShapes.Count > 0;
            
            foreach (var shape in (_drawnShapes))
            {
                var painter = Config.painterPrototypes[shape.Name];
                var item = painter.Draw(shape);
                canvas.Children.Add(item);
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {

           Command.executeCommand(new CutCommand(this));
           RedrawCanvas();
        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new PasteCommand(this));
            RedrawCanvas();
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
            int index = dashComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentDash = null;
                    break;
                case 1:
                    _currentDash = new DoubleCollection() { 4, 1, 1, 1, 1, 1 };
                    break;
                case 2:
                    _currentDash = new DoubleCollection() { 1, 1 };
                    break;
                case 3:
                    _currentDash = new DoubleCollection() { 6, 1 };
                    break;
                default:
                    break;
            }
        }

        private void sizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sizeComboBox.SelectedIndex;

            switch (index)
            {
                case 0:
                    _currentThickness = 1;
                    break;
                case 1:
                    _currentThickness = 2;
                    break;
                case 2:
                    _currentThickness = 3;
                    break;
                case 3:
                    _currentThickness = 5;
                    break;
                default:
                    break;
            }
        }

        private void btnBasicBlack_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void btnBasicOrange_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 165, 0));
        }

        private void btnBasicYellow_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        private void btnBasicBlue_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        }

        private void btnBasicGreen_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void btnBasicPurple_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(191, 64, 191));
        }

        private void btnBasicPink_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 182, 193));
        }

        private void btnBasicBrown_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(160, 82, 45));
        }

        private void btnBasicGray_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(192, 192, 192));
        }

        private void btnBasicRed_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        private void editColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorPicker = new System.Windows.Forms.ColorDialog();

            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _currentColor = new SolidColorBrush(Color.FromRgb(colorPicker.Color.R, colorPicker.Color.G, colorPicker.Color.B));
            }
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new CopyCommand(this));
        }


        private void border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _newStartPoint = e.GetPosition(canvas);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void zoom100Button_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new ZoomCommand(this, ZoomType.DEFAULT));
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new ZoomCommand(this, ZoomType.ZOOM_IN));
        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Command.executeCommand(new ZoomCommand(this, ZoomType.ZOOM_OUT));
        }

        private void uiZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Diagnostics.Debug.WriteLine(zoomRatio);
        }

        private void fillButton_Click(object sender, RoutedEventArgs e)
        {
            _isFilling = true;
        }

        private void insertItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                _backgroundImagePath = path;
                
                Image result1 = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(path, UriKind.Absolute));//source of image 
                result1.Source = bitmapImage;
                
                result1.Stretch = Stretch.Fill;

                Canvas.SetTop(result1, tmp_position.Y);
                Canvas.SetLeft(result1, tmp_position.X);
                _images.Add(result1 as Image);

                
                canvas.Children.Add(result1);
                
            }
        }

        private void MenuItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
