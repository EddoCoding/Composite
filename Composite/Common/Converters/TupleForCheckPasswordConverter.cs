using Composite.ViewModels.Notes.HardNote;
using System.Globalization;
using System.Windows.Data;

namespace Composite.Common.Converters
{
    public class TupleForCheckPasswordConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var note = values[0] as HardNoteVM;
            var Identifier = values[1] as string;

            return (note, Identifier);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}