using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteVM : NoteBaseVM
    {
        [ObservableProperty] string content;
        [ObservableProperty] DateTime dateCreate;
        [ObservableProperty] string fontFamily = "Times New Roman";
        [ObservableProperty] double fontSize = 18;
        public NoteVM() => Id = Guid.NewGuid();
    }
}