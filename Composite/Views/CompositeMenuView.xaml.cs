using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        void button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if(button.Parent is Grid grid)
            {
                var popup = grid.FindName("NotePopup") as Popup;
                if (popup != null) popup.IsOpen = !popup.IsOpen;
            }
            else if(button.Parent is StackPanel stackPanel)
            {
                var popup = stackPanel.FindName("NotePopup") as Popup;
                if (popup != null) popup.IsOpen = !popup.IsOpen;
            }    
            
        }
    }
}