using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace Composite.Common.Helpers
{
    public class MyDragHandler : IDragSource, IDropTarget
    {
        readonly Action<bool> _setDraggingState;

        public MyDragHandler() { }
        public MyDragHandler(Action<bool> setDraggingState) => _setDraggingState = setDraggingState;

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            DependencyObject src = Mouse.DirectlyOver as DependencyObject;

            var btn = FindParent<Button>(src);
            if (btn?.Name == "DragButton") return true;

            var tab = FindParent<TabItem>(src);
            if (tab != null) return true;

            return false;
        }
        public void StartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is TabItem tab) dragInfo.Data = tab.DataContext;
            else dragInfo.Data = dragInfo.SourceItem;

            dragInfo.Effects = DragDropEffects.Move;
            _setDraggingState?.Invoke(true);
        }
        public void DragCancelled() => _setDraggingState?.Invoke(false);
        public void Dropped(IDropInfo dropInfo) => _setDraggingState?.Invoke(false);
        public void DragDropOperationFinished(DragDropEffects op, IDragInfo dragInfo) => _setDraggingState?.Invoke(false);
        public bool TryCatchOccurredException(Exception exception) => false;
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = typeof(CustomInsertAdorner);
        }
        public void Drop(IDropInfo dropInfo) => DragDrop.DefaultDropHandler.Drop(dropInfo);
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