using IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineEntity
{
    public class LineEntity : IShapeEntity, ICloneable
    {
        public Point Start { get; set; }
        public Point End { get; set; }


        public string Name => "Line";

        public BitmapImage Icon => new BitmapImage(new Uri("Images/line.png", UriKind.Relative));

        public SolidColorBrush Brush { get; set; }
        public int Thickness { get; set; }
        public DoubleCollection StrokeDash { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void HandleEnd(Point point)
        {
            End = point;
        }

        public void HandleStart(Point point)
        {
            Start = point;
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
            // do nothing
        }

        public bool isHovering(double x, double y)
        {
            return Util.isBetween(x, Start.X, End.X) && Util.isBetween(y, Start.Y, End.Y);
        }

        public void pasteShape(Point startPoint, IShapeEntity shape)
        {
            var element = shape as LineEntity;

            Start = startPoint;
            var X = startPoint.X + element.End.X - element.Start.X;
            var Y = startPoint.Y + element.End.Y - element.Start.Y;
            Point endPoint = new Point(X, Y);
            End = endPoint;
        }
    }
}
