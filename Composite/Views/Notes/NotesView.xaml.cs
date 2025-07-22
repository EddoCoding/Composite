using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Composite.Views.Notes
{
    public partial class NotesView : UserControl
    {
        public NotesView() => InitializeComponent();

        void ContextMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                DependencyObject parent = button;
                while (parent != null && !(parent is ContextMenu)) parent = VisualTreeHelper.GetParent(parent);

                if (parent is ContextMenu contextMenu) contextMenu.IsOpen = false;
            }
        }
    }
}
