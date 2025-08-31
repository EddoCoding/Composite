using System.Windows;
using System.Windows.Input;

namespace Composite.Views.Notes
{
    public partial class AddCategoryView : Window
    {
        public AddCategoryView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
