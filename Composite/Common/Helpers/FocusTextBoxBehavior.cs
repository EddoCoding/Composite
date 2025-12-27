using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Composite.Common.Helpers
{
    public class FocusTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.IsVisible)
            {
                AssociatedObject.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AssociatedObject.Focus();
                    AssociatedObject.CaretIndex = AssociatedObject.Text?.Length ?? 0;
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            base.OnDetaching();
        }
    }
}