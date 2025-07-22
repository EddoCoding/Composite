using System.Globalization;
using System.Windows.Data;

namespace Composite.Common.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime) return dateTime.ToString("d MMMM HH:mm", new CultureInfo("ru-RU"));

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}