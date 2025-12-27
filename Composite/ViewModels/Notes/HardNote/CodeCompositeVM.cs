using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class CodeCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string _text = string.Empty;

        public CodeCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new CodeCompositeVM() { Id = Guid.NewGuid(), Text = Text };
        protected override void Dispose(bool disposing)
        {
            if (disposing) Text = string.Empty;
            base.Dispose(disposing);
        }
    }
}