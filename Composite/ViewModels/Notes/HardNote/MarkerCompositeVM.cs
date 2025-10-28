using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class MarkerCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;

        public MarkerCompositeVM() => Id = Guid.NewGuid();
        public override object Clone() => new MarkerCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
    }
}