using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public abstract partial class NoteBaseVM : ObservableObject
    {
        public Guid Id { get; set; }
        [ObservableProperty] string title = string.Empty;
    }
}