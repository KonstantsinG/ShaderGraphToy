using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ShaderGraphToy.Utilities.XamlConverters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToBrushConverter : IValueConverter
    {
        private Color _defaultColor = Colors.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color color)
                return new SolidColorBrush(_defaultColor);

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush brush)
                return _defaultColor;

            return brush.Color;
        }
    }
}
