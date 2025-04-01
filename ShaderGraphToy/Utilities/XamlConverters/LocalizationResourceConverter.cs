using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ShaderGraphToy.Utilities.XamlConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class LocalizationResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string locKey)
                return string.Empty;

            if (Application.Current.Resources.Contains(locKey))
                return Application.Current.Resources[locKey];
            else
                return locKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
