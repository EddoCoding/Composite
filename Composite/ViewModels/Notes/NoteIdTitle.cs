using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes
{
    public partial class NoteIdTitle : ObservableObject
    {
        public Guid Id { get; set; }
        [ObservableProperty] string _title = string.Empty;
    }
}