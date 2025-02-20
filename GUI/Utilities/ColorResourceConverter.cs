using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GUI.Utilities
{
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class ColorResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string colorKey)
                return Brushes.Transparent;

            if (Application.Current.Resources.Contains(colorKey))
            {
                var resource = Application.Current.Resources[colorKey];
                return resource switch
                {
                    SolidColorBrush brush => brush,
                    Color color => new SolidColorBrush(color),
                    _ => Brushes.Transparent
                };
            }

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(colorKey);
                return new SolidColorBrush(color);
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
