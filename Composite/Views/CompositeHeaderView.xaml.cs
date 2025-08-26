using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Composite.Views
{
    public partial class CompositeHeaderView : UserControl
    {
        public CompositeHeaderView() => InitializeComponent();

        private DispatcherTimer closeTimer;
        private const int CLOSE_DELAY_MS = 50; // Задержка перед закрытием

        void SongTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            StopCloseTimer();
            try
            {
                SongActionsPopup.IsOpen = true;
            }
            catch { }
        }

        void SongTextBlock_MouseLeave(object sender, MouseEventArgs e) => StartCloseTimer();

        void PopupBorder_MouseEnter(object sender, MouseEventArgs e) => StopCloseTimer();

        void PopupBorder_MouseLeave(object sender, MouseEventArgs e) => StartCloseTimer();

        void StartCloseTimer()
        {
            StopCloseTimer(); // На всякий случай останавливаем предыдущий

            closeTimer = new DispatcherTimer();
            closeTimer.Interval = TimeSpan.FromMilliseconds(CLOSE_DELAY_MS);
            closeTimer.Tick += (s, e) => {
                SongActionsPopup.IsOpen = false;
                StopCloseTimer();
            };
            closeTimer.Start();
        }

        void StopCloseTimer()
        {
            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer = null;
            }
        }
    }
}