using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IContract
{
    public interface IShapeEntity : ICloneable
    {
        string Name { get; }
        BitmapImage Icon { get; }

        void HandleStart(Point point);
        void HandleEnd(Point point);

        bool isHovering(double x, double y);
        void pasteShape(Point startPoint, IShapeEntity shape);
    }
}
