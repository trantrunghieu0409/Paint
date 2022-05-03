using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Paint.Converter
{
    internal class PercentageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float number = (float)value * 100;
            number = (float)Math.Round(number, 2);
            return number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double number = (double)value / 100;
            number = (float)Math.Round(number, 2);
            return number;
        }
    }
}
