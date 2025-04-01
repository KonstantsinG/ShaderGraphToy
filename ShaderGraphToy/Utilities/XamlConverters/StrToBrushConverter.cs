using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ShaderGraphToy.Utilities.XamlConverters
{
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class StrToBrushConverter : IValueConverter
    {
        private Color _defaultColor = Colors.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string colorKey)
                return new SolidColorBrush(_defaultColor);

            if (colorKey == string.Empty)
                return new SolidColorBrush(_defaultColor);

            if (Application.Current.Resources.Contains(colorKey))
            {
                var resource = Application.Current.Resources[colorKey];
                return resource switch
                {
                    SolidColorBrush brush => brush,
                    Color color => new SolidColorBrush(color),
                    _ => new SolidColorBrush(_defaultColor)
                };
            }

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(colorKey);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(_defaultColor);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush brush)
                return _defaultColor.ToString();

            return brush.Color.ToString();
        }
    }
}
