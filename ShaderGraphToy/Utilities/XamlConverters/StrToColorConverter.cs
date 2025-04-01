using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ShaderGraphToy.Utilities.XamlConverters
{
    [ValueConversion(typeof(Color), typeof(Color))]
    public class StrToColorConverter : IValueConverter
    {
        private Color _defaultColor = Colors.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string colorKey)
                return _defaultColor;

            if (colorKey == string.Empty)
                return _defaultColor;

            if (Application.Current.Resources.Contains(colorKey))
            {
                var resource = Application.Current.Resources[colorKey];
                return resource switch
                {
                    SolidColorBrush brush => brush.Color,
                    Color color => color,
                    _ => _defaultColor
                };
            }

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(colorKey);
                return color;
            }
            catch
            {
                return _defaultColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color color)
                return _defaultColor.ToString();

            return color.ToString();
        }
    }
}
