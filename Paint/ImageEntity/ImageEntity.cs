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
        public float Width;
        public float Height;

        public string Name => "Image";

        public BitmapImage Icon => new BitmapImage(new Uri("Images/insert.png", UriKind.Relative));

        public BitmapImage Image;

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
            // do nothing 
        }

        public void HandleSolidColorBrush(SolidColorBrush brush)
        {
            // do nothing 
        }
        public void HandleThickness(int thickness)
        {
            // do nothing 
        }
        public void HandleDoubleCollection(DoubleCollection dash)
        {
            // do nothing 
        }

        public void HandleBackground(SolidColorBrush background)
        {
            // do nothing 
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
