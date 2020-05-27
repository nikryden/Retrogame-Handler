using System;
using System.Globalization;
using System.Windows.Data;

namespace RetroGameHandler.Converter
{
    internal class ByteSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long ln = (long)value;
            return SizeSuffix(ln, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string SizeSuffix(long value, int decimalPlaces = 0)
        {
            if (value < 0)
            {
                return "";
            }
            var mag = (int)Math.Max(0, Math.Log(value, 1024));
            var adjustedSize = Math.Round(value / Math.Pow(1024, mag), decimalPlaces);
            return $"{adjustedSize} {SizeSuffixes[mag]}";
        }
    }
}