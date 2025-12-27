using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Composite.Common.Helpers
{
    public class CustomInsertAdorner : DropTargetAdorner
    {
        public CustomInsertAdorner(UIElement adornedElement, IDropInfo dropInfo) : base(adornedElement, dropInfo) { }
        protected override void OnRender(DrawingContext dc)
        {
            var items = DropInfo.VisualTarget as ItemsControl;
            if (items == null) return;

            bool isTabControl = HasTabPanel(items);

            if (isTabControl)
                DrawVerticalLineForTabs(dc, items);
            else
                DrawHorizontalLineForList(dc, items);
        }

        // ---------------- TABCONTROL ------------------------------------
        void DrawVerticalLineForTabs(DrawingContext dc, ItemsControl items)
        {
            double x = GetInsertX(items, DropInfo.InsertIndex);

            double top = 2;
            double bottom = AdornedElement.RenderSize.Height - 2;

            var pen = new Pen(Brushes.Gray, 0.6);
            dc.DrawLine(pen, new Point(x, top), new Point(x, bottom));
        }
        double GetInsertX(ItemsControl items, int index)
        {
            var tabPanel = FindChild<TabPanel>(items);
            if (tabPanel == null) return 0;

            // Если перетаскиваем в начало
            if (index == 0)
            {
                var first = tabPanel.Children[0] as FrameworkElement;
                return first.TransformToVisual(AdornedElement).Transform(new Point(0, 0)).X;
            }

            // В конец
            if (index >= tabPanel.Children.Count)
            {
                var last = tabPanel.Children[tabPanel.Children.Count - 1] as FrameworkElement;
                var pos = last.TransformToVisual(AdornedElement).Transform(new Point(0, 0));
                return pos.X + last.ActualWidth;
            }

            // Между элементами
            var container = tabPanel.Children[index] as FrameworkElement;
            return container.TransformToVisual(AdornedElement).Transform(new Point(0, 0)).X;
        }

        // ---------------- LISTVIEW / VERTICAL LIST ----------------------
        void DrawHorizontalLineForList(DrawingContext dc, ItemsControl items)
        {
            double y = GetInsertY(items, DropInfo.InsertIndex);
            var pen = new Pen(Brushes.LightGray, 0.6);
            dc.DrawLine(pen, new Point((AdornedElement.RenderSize.Width - 1000) / 2, y), new Point((AdornedElement.RenderSize.Width - 1000) / 2 + 1000, y));
        }
        double GetInsertY(ItemsControl items, int index)
        {
            var container = items.ItemContainerGenerator.ContainerFromIndex(Math.Min(index, items.Items.Count - 1)) as FrameworkElement;

            if (container == null) return 0;

            var pos = container.TransformToVisual(AdornedElement).Transform(new Point(0, 0));

            if (index >= items.Items.Count) return pos.Y + container.ActualHeight;

            return pos.Y;
        }

        // ---------------- HELPERS ---------------------------------------
        bool HasTabPanel(ItemsControl items) => FindChild<TabPanel>(items) != null;
        T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t) return t;

                var result = FindChild<T>(child);
                if (result != null) return result;
            }

            return null;
        }
    }
}