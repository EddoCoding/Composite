using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                if (e.Delta > 0)
                {
                    scrollViewer.LineLeft();
                    scrollViewer.LineLeft();
                    scrollViewer.LineLeft();
                }
                else
                {
                    scrollViewer.LineRight();
                    scrollViewer.LineRight();
                    scrollViewer.LineRight();
                }

                e.Handled = true;
            }
        }
    }
}
