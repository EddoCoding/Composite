using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HeaderCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string header = string.Empty;

        public HeaderCompositeVM()
        {
            Id = Guid.NewGuid();
        }
    }
}