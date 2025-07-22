using System.Windows;
using System.Windows.Input;

namespace Composite.Views.Notes
{
    public partial class SetPasswordView : Window
    {
        public SetPasswordView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
