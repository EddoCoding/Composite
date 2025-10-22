using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Composite.Common.Converters
{
    public class EditingAndHoverToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Visibility.Hidden;

            bool isEditing = values[0] is bool b1 && b1;
            bool isHover = values[1] is bool b2 && b2;

            return (!isEditing && isHover) ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}