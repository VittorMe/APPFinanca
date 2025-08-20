using System.Globalization;

namespace APPFinanca.Libraries.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public static StringToVisibilityConverter Instance { get; } = new StringToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 