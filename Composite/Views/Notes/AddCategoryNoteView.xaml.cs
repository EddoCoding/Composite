using System.Windows;
using System.Windows.Input;

namespace Composite.Views.Notes
{
    public partial class AddCategoryNoteView : Window
    {
        public AddCategoryNoteView()
        {
            InitializeComponent();
            textBox.Focus();
        }

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
