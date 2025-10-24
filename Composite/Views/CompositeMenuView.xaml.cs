using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Composite.Views
{
    public partial class CompositeMenuView : UserControl
    {
        public CompositeMenuView() => InitializeComponent();

        void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                DependencyObject parent = button;
                while (parent != null && !(parent is Popup)) parent = VisualTreeHelper.GetParent(parent);

                if (parent is Popup popup) popup.IsOpen = false;
                else
                {
                    var popupFromLogical = FindLogicalParent<Popup>(button);
                    if (popupFromLogical != null) popupFromLogical.IsOpen = false;
                }
            }
        }
        T FindLogicalParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(child);

            if (parent == null) return null;
            if (parent is T typedParent) return typedParent;

            return FindLogicalParent<T>(parent);
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