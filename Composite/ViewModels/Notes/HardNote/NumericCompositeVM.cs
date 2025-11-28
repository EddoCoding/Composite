using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class NumericCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] int _number;
        [ObservableProperty] string _text = string.Empty;

        public NumericCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new NumericCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text};
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Number = 0;
                Text = string.Empty;
            }
            base.Dispose(disposing);
        }
    }
}