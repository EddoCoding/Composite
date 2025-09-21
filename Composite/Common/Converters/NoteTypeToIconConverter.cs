using System.Globalization;
using System.Windows.Data;

namespace Composite.Common.Converters
{
    public class NoteTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "/Common/Images/noteImage.png";

            switch (value.ToString())
            {
                case "Note":
                    return "/Common/Images/noteImage.png";
                case "HardNote":
                    return "/Common/Images/hardNoteImage.png";
                default:
                    return "/Common/Images/noteImage.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
