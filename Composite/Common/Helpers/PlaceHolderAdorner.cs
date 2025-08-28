using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Data;

namespace Composite.Common.Helpers
{
    public class PlaceHolderAdorner : Adorner
    {
        readonly TextBlock _placeholderText;

        public PlaceHolderAdorner(UIElement adornedElement, string placeholder) : base(adornedElement)
        {
            _placeholderText = new TextBlock
            {
                Text = placeholder,
                Foreground = Brushes.LightGray,
                Margin = new Thickness(3,0,0,0),
                IsHitTestVisible = false
            };

            _placeholderText.SetBinding(TextBlock.FontSizeProperty, new Binding("FontSize") { Source = adornedElement });
            _placeholderText.SetBinding(TextBlock.FontFamilyProperty, new Binding("FontFamily") { Source = adornedElement });
            _placeholderText.SetBinding(TextBlock.FontStyleProperty, new Binding("FontStyle") { Source = adornedElement });
            _placeholderText.SetBinding(TextBlock.FontWeightProperty, new Binding("FontWeight") { Source = adornedElement });

            AddVisualChild(_placeholderText);
        }
        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _placeholderText;
        protected override Size ArrangeOverride(Size finalSize)
        {
            _placeholderText.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}