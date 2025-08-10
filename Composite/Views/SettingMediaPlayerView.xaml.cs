using System.Windows;
using System.Windows.Input;

namespace Composite.Views
{
    public partial class SettingMediaPlayerView : Window
    {
        public SettingMediaPlayerView() => InitializeComponent();

        void MoveWindow(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}