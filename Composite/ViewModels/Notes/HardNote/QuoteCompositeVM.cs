using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class QuoteCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string _text = string.Empty;

        public QuoteCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new QuoteCompositeVM() { Id = Guid.NewGuid(), Text = Text };
        protected override void Dispose(bool disposing)
        {
            if (disposing) Text = string.Empty;
            base.Dispose(disposing);
        }
    }
}