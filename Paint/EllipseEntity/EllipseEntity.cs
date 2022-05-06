using IContract;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EllipseEntity
{
    public class EllipseEntity : IShapeEntity, ICloneable
    {
        public Point TopLeft { get; set; }
        public Point RightBottom { get; set; }

        public string Name => "Ellipse";

        public BitmapImage Icon => new BitmapImage(new Uri("Images/ellipse.png", UriKind.Relative));

        BitmapImage IShapeEntity.Icon => throw new NotImplementedException();

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
        public void HandleBackground(SolidColorBrush background)
        {
            Background = background;
        }

        public object Clone()
        {
            EllipseEntity cloneShape = (EllipseEntity)this.MemberwiseClone();
            if (Brush != null) cloneShape.Brush = Brush.Clone();
            if (Background != null) cloneShape.Background = Background.Clone();
            if (StrokeDash != null) cloneShape.StrokeDash = StrokeDash.Clone();
            return cloneShape;
        }

        public bool isHovering(double x, double y)
        {
            return Util.isBetween(x, TopLeft.X, RightBottom.X) && Util.isBetween(y, TopLeft.Y, RightBottom.Y);
        }
        public void pasteShape(Point startPoint, IShapeEntity shape)
        {
            var element = shape as EllipseEntity;

            TopLeft = startPoint;
            var X = startPoint.X + Math.Abs(element!.RightBottom.X - element.TopLeft.X);
            var Y = startPoint.Y + Math.Abs(element.RightBottom.Y - element.TopLeft.Y);
            Point endPoint = new Point(X, Y);
            RightBottom = endPoint;
        }
    }
}
