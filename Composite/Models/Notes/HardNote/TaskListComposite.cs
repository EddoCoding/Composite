namespace Composite.Models.Notes.HardNote
{
    public class TaskListComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Completed { get; set; }

        public TaskListComposite()
        {
            CompositeType = nameof(TaskListComposite);
            Children = new List<CompositeBase>();
        }
    }
}