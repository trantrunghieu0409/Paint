using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IContract
{
    public interface IShapeEntity : ICloneable
    {
        string Name { get; }
        BitmapImage Icon { get; }

        void HandleStart(Point point);
        void HandleEnd(Point point);

        void HandleSolidColorBrush(SolidColorBrush brush);
        void HandleThickness(int thickness);
        void HandleDoubleCollection(DoubleCollection dash);
        void HandleBackground(SolidColorBrush background);

        bool isHovering(double x, double y);
        void pasteShape(Point startPoint, IShapeEntity shape);
    }
}
