using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string text = string.Empty;

        public TextCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new TextCompositeVM() { Id = Guid.NewGuid(), Text = Text };
    }
}