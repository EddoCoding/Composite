using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string text = string.Empty;

        public TextCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new TextCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
            }
            base.Dispose(disposing);
        }
    }
}