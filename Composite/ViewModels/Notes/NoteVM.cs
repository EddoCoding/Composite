using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteVM : NoteBaseVM
    {
        [ObservableProperty] string content;
        [ObservableProperty] DateTime dateCreate;
        [ObservableProperty] string password = string.Empty;
        [ObservableProperty] bool preview;

        public NoteVM() => Id = Guid.NewGuid();
    }
}