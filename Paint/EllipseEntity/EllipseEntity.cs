using IContract;
using System;
using System.Windows;
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

        public void HandleStart(Point point)
        {
            TopLeft = point;
        }
        public void HandleEnd(Point point)
        {
            RightBottom = point;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
