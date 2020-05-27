using System;

using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RetroGameHandler.Converter
{
    public class BoolColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? "#FFFFFF" : "#adadad";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string)value;
            var col = ViewModels.RetroResorcesViewModel.Categories.FirstOrDefault(c => c.category == val)?.color??"blue";
          
            if (col.StartsWith("#"))
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}