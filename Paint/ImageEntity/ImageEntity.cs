using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEntity
{
    public class ImageEntity : IShapeEntity, ICloneable
    {
        public Point TopLeft { get; set; }
        public Point RightBottom { get; set; }

        public float Width;
        public float Height;

        public string Name => "Image";

        public BitmapImage Icon => new BitmapImage(new Uri("Images/insert.png", UriKind.Relative));

        public BitmapImage Image { get; set; }

        public SolidColorBrush Brush { get; set; }
        public SolidColorBrush Background { get; set; }
        public int Thickness { get; set; }
        public DoubleCollection StrokeDash { get; set; }

        
        public void HandleStart(Point point)
        {
            TopLeft = point;
        }
        public void HandleEnd(Point point)
        {
            
        }

        public void HandleSolidColorBrush(SolidColorBrush brush)
        {
            Brush = brush;
        }
        public void HandleThickness(int thickness)
        {
            Thickness = thickness;
        }
        public void HandleDoubleCollection(DoubleCollection dash)
        {
            StrokeDash = dash;
        }

        public void HandleBackground(SolidColorBrush background)
        {
            Background = background;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public bool isHovering(double x, double y)
        {
            return false;
        }

        public void pasteShape(Point startPoint, IShapeEntity shape)
        {
            var element = shape as ImageEntity;

            TopLeft = startPoint;
            
        }


    }
}
