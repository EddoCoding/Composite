using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class NumericCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] int _number;
        [ObservableProperty] string _text = string.Empty;

        public NumericCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new NumericCompositeVM() { Id = Guid.NewGuid(), Text = Text};
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Number = 0;
                Text = string.Empty;
            }
            base.Dispose(disposing);
        }
    }
}