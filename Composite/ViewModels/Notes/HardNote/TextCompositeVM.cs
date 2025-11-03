using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextCompositeVM : CompositeBaseVM, IDisposable
    {
        public event EventHandler<bool>? TextStartsWithSlashChanged;

        [ObservableProperty] string text = string.Empty;

        public TextCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new TextCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };

        partial void OnTextChanged(string value)
        {
            bool startsWithSlash = !string.IsNullOrEmpty(value) && value[0] == '/';
            TextStartsWithSlashChanged?.Invoke(this, startsWithSlash);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
                TextStartsWithSlashChanged = null;
            }
            base.Dispose(disposing);
        }
    }
}