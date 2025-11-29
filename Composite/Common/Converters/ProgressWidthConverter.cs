using System.Globalization;
using System.Windows.Data;

namespace Composite.Common.Converters
{
    public class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double currentValue && values[1] is double maxValue && values[2] is double trackWidth)
            {
                if (maxValue == 0) return 0.0;
                return (currentValue / maxValue) * trackWidth;
            }
            return 0.0;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}