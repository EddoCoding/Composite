namespace Composite.Models.Notes.HardNote
{
    public class TaskComposite : CompositeBase
    {
        public int Completed { get; set; }
        public string Text { get; set; } = string.Empty;
        public TaskComposite() => CompositeType = nameof(TaskComposite);
    }
}