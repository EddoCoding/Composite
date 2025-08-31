using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public abstract class CompositeBaseVM : ObservableObject
    {
        public Guid Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}