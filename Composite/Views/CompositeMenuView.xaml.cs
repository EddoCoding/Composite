using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Composite.Views
{
    public partial class CompositeMenuView : UserControl
    {
        public CompositeMenuView() => InitializeComponent();

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