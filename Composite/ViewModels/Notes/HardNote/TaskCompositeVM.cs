using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TaskCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] bool _isCompleted;

        public TaskCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new TaskCompositeVM() { Id = Guid.NewGuid(), Text = Text, IsCompleted = IsCompleted };
    }
}