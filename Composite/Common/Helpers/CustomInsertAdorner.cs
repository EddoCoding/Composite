using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Composite.Common.Helpers
{
    public class CustomInsertAdorner : DropTargetAdorner
    {
        public CustomInsertAdorner(UIElement adornedElement, IDropInfo dropInfo) : base(adornedElement, dropInfo) { }

        protected override void OnRender(DrawingContext dc)
        {
            var items = DropInfo.VisualTarget as ItemsControl;
            if (items == null || items.Items.Count == 0) return;

            int index = DropInfo.InsertIndex;

            double y;

            if (index == 0)
            {
                var first = items.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                if (first != null)
                {
                    var transform = first.TransformToVisual(AdornedElement);
                    var pos = transform.Transform(new Point(0, 0));
                    y = pos.Y;
                }
                else y = 0;
            }
            else if (index >= items.Items.Count)
            {
                var last = items.ItemContainerGenerator.ContainerFromIndex(items.Items.Count - 1) as FrameworkElement;
                if (last != null)
                {
                    var transform = last.TransformToVisual(AdornedElement);
                    var pos = transform.Transform(new Point(0, 0));
                    y = pos.Y + last.ActualHeight;
                }
                else y = items.RenderSize.Height;
            }
            else
            {
                var container = items.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                if (container != null)
                {
                    var transform = container.TransformToVisual(AdornedElement);
                    var pos = transform.Transform(new Point(0, 0));
                    y = pos.Y;
                }
                else y = 0;
            }

            var pen = new Pen(Brushes.LightGray, 0.6);
            dc.DrawLine(pen, new Point((AdornedElement.RenderSize.Width - 1000) / 2, y), new Point((AdornedElement.RenderSize.Width - 1000) / 2 + 1000, y) );
        }
    }
}