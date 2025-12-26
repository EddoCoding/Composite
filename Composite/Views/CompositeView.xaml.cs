using System.Windows;
using System.Windows.Input;

namespace Composite.Views
{
    public partial class CompositeView : Window
    {
        public CompositeView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}