using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TextComposite : CompositeBase
    {
        [ObservableProperty] string text = string.Empty;
    }
}