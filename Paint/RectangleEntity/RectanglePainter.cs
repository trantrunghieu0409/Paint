using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectangleEntity
{
    public class IRectanglePainter : IPaintBusiness
    {
        public UIElement Draw(IShapeEntity shape)
        {
            var rectangle = shape as RectangleEntity;

            // TODO: chú ý việc đảo lại rightbottom và topleft 
            double width = rectangle.RightBottom.X - rectangle.TopLeft.X;
            double height = rectangle.RightBottom.Y - rectangle.TopLeft.Y;

            var element = new Rectangle()
            {
                Width = width,
                Height = height,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(element, rectangle.TopLeft.X);
            Canvas.SetTop(element, rectangle.TopLeft.Y);

            return element;
        }
    }
}
