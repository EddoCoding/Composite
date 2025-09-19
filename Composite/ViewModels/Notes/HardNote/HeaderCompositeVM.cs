using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HeaderCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] string _fontWeight = string.Empty;
        [ObservableProperty] double _fontSize;

        public HeaderCompositeVM() => Id = Guid.NewGuid();
    }
}