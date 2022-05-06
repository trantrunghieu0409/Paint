using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RectangleEntity
{
    public class RectangleEntity : IShapeEntity, ICloneable
    {
        public Point TopLeft { get; set; }
        public Point RightBottom { get; set; }

        public string Name => "Rectangle";

        public BitmapImage Icon => new BitmapImage(new Uri("Images/rectangle.png", UriKind.Relative));

        public SolidColorBrush Brush { get; set; }
        public int Thickness { get; set; }
        public DoubleCollection StrokeDash { get; set; }

        public void HandleStart(Point point)
        {
            TopLeft = point;
        }
        public void HandleEnd(Point point)
        {
            RightBottom = point;
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

        public object Clone()
        {
            return MemberwiseClone();
        }
        public bool isHovering(double x, double y)
        {
            return Util.isBetween(x, TopLeft.X, RightBottom.X) && Util.isBetween(y, TopLeft.Y, RightBottom.Y);
        }

        public void pasteShape(Point startPoint, IShapeEntity shape)
        {
            var element = shape as RectangleEntity;

            TopLeft = startPoint;
            var X = startPoint.X + Math.Abs(element!.RightBottom.X - element.TopLeft.X);
            var Y = startPoint.Y + Math.Abs(element.RightBottom.Y - element.TopLeft.Y);
            Point endPoint = new Point(X, Y);
            RightBottom = endPoint;
        }
    }
}
