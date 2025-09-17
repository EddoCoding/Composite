using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteVM : NoteBaseVM
    {
        [ObservableProperty] string content;
        [ObservableProperty] DateTime dateCreate;
        [ObservableProperty] string password = string.Empty;
        [ObservableProperty] string fontFamily = "Times New Roman";
        [ObservableProperty] double fontSize = 18;
        [ObservableProperty] string color = string.Empty;
        public NoteVM() => Id = Guid.NewGuid();
    }
}