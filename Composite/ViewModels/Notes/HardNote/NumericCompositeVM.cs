using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class NumericCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] int _number;
        [ObservableProperty] string _text = string.Empty;

        public NumericCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new HeaderCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text};
    }
}