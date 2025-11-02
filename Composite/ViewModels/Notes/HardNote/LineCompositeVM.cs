using static System.Net.Mime.MediaTypeNames;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class LineCompositeVM : CompositeBaseVM, IDisposable
    {
        public LineCompositeVM() => Id = Guid.NewGuid();
        public override object Clone() => new LineCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment };

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
            }
            base.Dispose(disposing);
        }
    }
}