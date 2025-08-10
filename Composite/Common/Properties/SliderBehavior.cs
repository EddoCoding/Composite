using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Composite.Common.Properties
{
    public class SliderBehavior
    {
        public static readonly DependencyProperty StartSeekCommandProperty =
            DependencyProperty.RegisterAttached("StartSeekCommand", typeof(ICommand), typeof(SliderBehavior), new PropertyMetadata(null, OnStartSeekCommandChanged));

        public static readonly DependencyProperty StopSeekCommandProperty =
            DependencyProperty.RegisterAttached("StopSeekCommand", typeof(ICommand), typeof(SliderBehavior), new PropertyMetadata(null, OnStopSeekCommandChanged));

        public static ICommand GetStartSeekCommand(DependencyObject obj) => (ICommand)obj.GetValue(StartSeekCommandProperty);
        public static void SetStartSeekCommand(DependencyObject obj, ICommand value) => obj.SetValue(StartSeekCommandProperty, value);
        public static ICommand GetStopSeekCommand(DependencyObject obj) => (ICommand)obj.GetValue(StopSeekCommandProperty);
        public static void SetStopSeekCommand(DependencyObject obj, ICommand value) => obj.SetValue(StopSeekCommandProperty, value);
        static void OnStartSeekCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                if (e.OldValue != null) slider.PreviewMouseDown -= OnSliderPreviewMouseDown;
                if (e.NewValue != null) slider.PreviewMouseDown += OnSliderPreviewMouseDown;
            }
        }
        static void OnStopSeekCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                if (e.OldValue != null) slider.PreviewMouseUp -= OnSliderPreviewMouseUp;
                if (e.NewValue != null) slider.PreviewMouseUp += OnSliderPreviewMouseUp;
            }
        }
        static void OnSliderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                var command = GetStartSeekCommand(slider);
                command?.Execute(null);
            }
        }
        static void OnSliderPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                var command = GetStopSeekCommand(slider);
                command?.Execute(slider.Value);
            }
        }
    }
}
