using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string text = string.Empty;
        [ObservableProperty] bool _isOpenSnippetsPopup;

        public TextCompositeVM() => Id = Guid.NewGuid();

        partial void OnTextChanged(string value) => IsOpenSnippetsPopup = !string.IsNullOrEmpty(value) && value[0] == '/';

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