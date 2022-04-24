﻿using IContract;
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

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        // State
        bool _isDrawing = false;
        string _currentType = "";
        IShapeEntity _preview;
        Point _start;
        List<IShapeEntity> _drawnShapes = new List<IShapeEntity>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Config.firstConfig();

            Title = $"Tìm thấy {Config.shapesPrototypes.Count} hình";

            // Tạo ra các nút bấm tương ứng
            foreach (var (name, entity) in Config.shapesPrototypes)
            {
                var button = new Button();
                button.Content = name;
                button.Tag = entity;
                button.Width = 80;
                button.Height = 35;
                button.Click += Button_Click;

                //TODO: thêm các nút bấm vào giao diện
                actionsStackPanel.Children.Add(button);
            }

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

        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void iconListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
    }
}
