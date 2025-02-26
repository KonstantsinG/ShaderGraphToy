using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace GUI.Utilities.Converters
{
    [ValueConversion(typeof(IList), typeof(string))]
    public class ListIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList list && parameter is string strparam && int.TryParse(strparam, out int index) && index < list.Count)
                return list[index] ?? string.Empty;

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
