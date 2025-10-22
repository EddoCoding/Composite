using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public abstract partial class CompositeBaseVM : ObservableObject, ICloneable
    {
        public Guid Id { get; set; }
        [ObservableProperty] string _tag;
        [ObservableProperty] string _comment;
        [ObservableProperty] bool _isEditing;
        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        public abstract object Clone();
    }
}