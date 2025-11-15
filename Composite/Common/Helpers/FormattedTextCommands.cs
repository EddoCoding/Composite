using System.Windows.Input;

namespace Composite.Common.Helpers
{
    public class FormattedTextCommands
    {
        public static readonly RoutedUICommand ToggleStrikethrough = new RoutedUICommand("Toggle Strikethrough", "ToggleStrikethrough", typeof(FormattedTextCommands));
        public static readonly RoutedUICommand ToggleOverline = new RoutedUICommand("Toggle Overline", "ToggleOverline", typeof(FormattedTextCommands));
        public static readonly RoutedUICommand ChangeFont = new RoutedUICommand("Change Font", "ChangeFont", typeof(FormattedTextCommands));
        public static readonly RoutedUICommand ChangeSize = new RoutedUICommand("Change Size", "ChangeSize", typeof(FormattedTextCommands));
        public static readonly RoutedUICommand ChangeTextColor = new RoutedUICommand("Change TextColor", "ChangeTextColor", typeof(FormattedTextCommands));
        public static readonly RoutedUICommand ChangeBgTextColor = new RoutedUICommand("Change BgTextColor", "ChangeBgTextColor", typeof(FormattedTextCommands));
    }
}