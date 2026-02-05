using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ElectricApp
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double size && parameter is string paramString)
            {
                double factor;
                if (double.TryParse(paramString, NumberStyles.Any, CultureInfo.InvariantCulture, out factor))
                {
                    return size * factor;
                }
                return size * 0.02; // значение по умолчанию
            }
            return 16.0; // размер по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}