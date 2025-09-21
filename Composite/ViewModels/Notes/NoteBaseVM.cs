using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public abstract partial class NoteBaseVM : ObservableObject
    {
        public Guid Id { get; set; }
        public virtual string ItemType => string.Empty;
        [ObservableProperty] string title = string.Empty;
        [ObservableProperty] string category = string.Empty;
        public DateTime DateCreate { get; set; }
    }
}