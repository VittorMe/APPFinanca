using System.Globalization;
using static APPFinanca.Models.Enumation;

namespace APPFinanca.Libraries.Converters
{
    public class MonthToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Month month)
            {
                return month.ToString(); // Converte para o nome do mês
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.TryParse<Month>(value.ToString(), out var month) ? month : Month.Janeiro;
        }
    }
}
