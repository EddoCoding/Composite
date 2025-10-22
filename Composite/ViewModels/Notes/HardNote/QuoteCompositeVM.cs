using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class QuoteCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;

        public QuoteCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new QuoteCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
    }
}