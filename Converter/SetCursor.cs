using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace RetroGameHandler.Converter
{
    internal class SetCursor : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)values;
            return val ? Cursors.Arrow : Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}