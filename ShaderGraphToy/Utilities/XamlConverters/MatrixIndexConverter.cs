using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace ShaderGraphToy.Utilities.XamlConverters
{
    [ValueConversion(typeof(IList), typeof(string))]
    public class MatrixIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IList mainList || parameter is not string paramStr)
                return string.Empty;

            string[] indexes = paramStr.Split(',');
            if (indexes.Length != 2)
                return string.Empty;

            if (!int.TryParse(indexes[0], out int firstIndex) ||
                !int.TryParse(indexes[1], out int secondIndex))
                return string.Empty;

            if (firstIndex < 0 || firstIndex >= mainList.Count)
                return string.Empty;

            if (mainList[firstIndex] is not IList nestedList)
                return string.Empty;

            if (secondIndex < 0 || secondIndex >= nestedList.Count)
                return string.Empty;

            return nestedList[secondIndex]?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
