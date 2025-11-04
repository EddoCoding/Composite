using System.Windows;

namespace Composite.Common.Properties
{
    public class ButtonHelper
    {
        public static readonly DependencyProperty AdditionalTextProperty = DependencyProperty.RegisterAttached("AdditionalText", 
            typeof(string), typeof(ButtonHelper), new PropertyMetadata(string.Empty));

        public static string GetAdditionalText(DependencyObject obj) => (string)obj.GetValue(AdditionalTextProperty);
        public static void SetAdditionalText(DependencyObject obj, string value) => obj.SetValue(AdditionalTextProperty, value);
    }
}