using System.Windows;
using System.Windows.Input;

namespace Composite.Views.Notes
{
    public partial class SelectTypeNoteView : Window
    {
        public SelectTypeNoteView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
