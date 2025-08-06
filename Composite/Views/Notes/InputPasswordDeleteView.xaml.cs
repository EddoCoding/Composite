using System.Windows;
using System.Windows.Input;

namespace Composite.Views.Notes
{
    public partial class InputPasswordDeleteView : Window
    {
        public InputPasswordDeleteView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
