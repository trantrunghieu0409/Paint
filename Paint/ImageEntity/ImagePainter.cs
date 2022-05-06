using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageEntity
{
    public class IImagePainter : IPaintBusiness
    {
        public UIElement Draw(IShapeEntity shape)
        {
            var image = shape as ImageEntity;

            // TODO: chú ý việc đảo lại rightbottom và topleft 

            while(image.Image == null)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "PNG (*.png)|*.png| JPEG (*.jpeg)|*.jpeg| BMP (*.bmp)|*.bmp";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;


                    BitmapImage bitmapImage = new BitmapImage(new Uri(path, UriKind.Absolute));//source of image 

                    image.Image = bitmapImage;
                }
            }       


            double width = image.Image.Width;
            double height = image.Image.Height;

            var element = new Image()
            {
                Width = width,
                Height = height,
                Source = image.Image
                //StrokeThickness = 1,
                //Stroke = new System.Windows.Media.SolidColorBrush(Colors.Red)

            };

            Canvas.SetTop(element, image.TopLeft.Y);
            Canvas.SetLeft(element, image.TopLeft.X);

            return element;          

            
            
        }
    }
}
