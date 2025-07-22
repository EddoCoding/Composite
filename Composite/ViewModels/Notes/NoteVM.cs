using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteVM : NoteBaseVM
    {
        public string Content { get; set; }
        [ObservableProperty] DateTime dateCreate;
        [ObservableProperty] string password = string.Empty;
        [ObservableProperty] bool preview;

        public NoteVM() => Id = Guid.NewGuid();
    }
}