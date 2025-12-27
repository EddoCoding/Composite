using Composite.Common.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Composite.Common.Properties
{
    public class PlaceHolder
    {
        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(PlaceHolder), new PropertyMetadata(string.Empty, OnPlaceHolderChanged));
        public static void SetPlaceHolder(UIElement element, string value) => element.SetValue(PlaceHolderProperty, value);
        public static string GetPlaceHolder(UIElement element) => (string)element.GetValue(PlaceHolderProperty);

        public static readonly DependencyProperty ShowAlwaysProperty = DependencyProperty.RegisterAttached("ShowAlways", typeof(bool), typeof(PlaceHolder), 
            new PropertyMetadata(false, (d, e) =>
            {
                if (d is TextBox tb) ShowOrHidePlaceholder(tb);
            }));

        public static void SetShowAlways(UIElement element, bool value) => element.SetValue(ShowAlwaysProperty, value);
        public static bool GetShowAlways(UIElement element) => (bool)element.GetValue(ShowAlwaysProperty);

        static void OnPlaceHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.Loaded += (s, _) => ShowOrHidePlaceholder(textBox);
            textBox.TextChanged += (s, _) => ShowOrHidePlaceholder(textBox);
            textBox.MouseEnter += (s, _) => ShowOrHidePlaceholder(textBox);
            textBox.MouseLeave += (s, _) => ShowOrHidePlaceholder(textBox);
        }
        static void ShowOrHidePlaceholder(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var existing = layer.GetAdorners(textBox);
            bool isEmpty = string.IsNullOrEmpty(textBox.Text);
            bool showAlways = GetShowAlways(textBox);
            bool shouldShow = isEmpty && (showAlways || textBox.IsMouseOver);

            if (!shouldShow)
            {
                if (existing != null)
                {
                    foreach (var adorner in existing)
                        if (adorner is PlaceHolderAdorner) layer.Remove(adorner);
                }
            }
            else
            {
                if (existing == null || !Array.Exists(existing, a => a is PlaceHolderAdorner))
                {
                    layer.Add(new PlaceHolderAdorner(textBox, GetPlaceHolder(textBox)));
                }
            }
        }
    }
}