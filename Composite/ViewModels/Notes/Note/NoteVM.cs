using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteVM : NoteBaseVM
    {
        public override string ItemType => "Note";
        [ObservableProperty] string content;
        [ObservableProperty] string fontFamily = "Times New Roman";
        [ObservableProperty] double fontSize = 18;
        public NoteVM() => Id = Guid.NewGuid();
    }
}