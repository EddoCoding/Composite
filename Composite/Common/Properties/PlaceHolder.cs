using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Composite.Common.Helpers;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Properties
{
    public class PlaceHolder
    {
        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(PlaceHolder), new PropertyMetadata(string.Empty, OnPlaceHolderChanged));
        public static void SetPlaceHolder(UIElement element, string value) => element.SetValue(PlaceHolderProperty, value);
        public static string GetPlaceHolder(UIElement element) => (string)element.GetValue(PlaceHolderProperty);
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
            bool isTextComposite = textBox.DataContext is TextComposite;
            bool isHeaderComposite = textBox.DataContext is HeaderComposite;

            bool shouldShow = isEmpty && ((isTextComposite && textBox.IsMouseOver) || isHeaderComposite);

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
                    layer.Add(new PlaceHolderAdorner(textBox, GetPlaceHolder(textBox)));
            }
        }
    }
}