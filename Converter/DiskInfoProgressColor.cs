using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RetroGameHandler.Converter
{
    internal class DiskInfoProgressColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = "Green";
            var val = System.Convert.ToUInt32(value);
            if (val > 50) color = "Yellow";
            if (val > 90) color = "Red";
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}