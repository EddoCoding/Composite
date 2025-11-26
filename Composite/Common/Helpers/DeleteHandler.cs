using GongSolutions.Wpf.DragDrop;
using System.Collections;
using System.Windows;

namespace Composite.Common.Helpers
{
    public class DeleteHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        }
        public void Drop(IDropInfo dropInfo)
        {
            var item = dropInfo.Data;
            var sourceCollection = dropInfo.DragInfo?.SourceCollection;
            if (dropInfo.DragInfo.SourceCollection is IList list) list.Remove(item);
        }
    }
}