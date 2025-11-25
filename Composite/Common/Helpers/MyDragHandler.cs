using GongSolutions.Wpf.DragDrop;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Composite.Common.Helpers
{
    public class MyDragHandler : IDragSource
    {
        public bool CanStartDrag(IDragInfo dragInfo)
        {
            var source = Mouse.DirectlyOver as DependencyObject;
            var btn = FindParent<Button>(source);
            return btn?.Name == "DragButton";
        }
        public void StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Data = dragInfo.SourceItem;
            dragInfo.Effects = DragDropEffects.Move;
        }
        public void DragCancelled() { }
        public void Dropped(IDropInfo dropInfo) { }
        public void DragOver(IDropInfo dropInfo) { }
        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }
        public bool TryCatchOccurredException(Exception exception) => false;

        static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T t) return t;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
