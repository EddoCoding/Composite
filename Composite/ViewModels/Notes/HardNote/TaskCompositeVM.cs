using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class TaskCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] bool _isCompleted;

        public TaskCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new TaskCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text, IsCompleted = IsCompleted };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
                IsCompleted = false;
            }
            base.Dispose(disposing);
        }
    }
}