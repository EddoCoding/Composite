using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string text = string.Empty;
        [ObservableProperty] bool _isOpenSnippetsPopup;

        public TextCompositeVM() => Id = Guid.NewGuid();

        partial void OnTextChanged(string value) => IsOpenSnippetsPopup = !string.IsNullOrEmpty(value) && value[0] == '/';

        public override object Clone() => new TextCompositeVM() { Id = Guid.NewGuid(), Text = Text };
        protected override void Dispose(bool disposing)
        {
            if (disposing) Text = string.Empty;
            base.Dispose(disposing);
        }
    }
}