namespace Composite.Models.Notes.HardNote
{
    public class SubTaskComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public int Completed { get; set; }

        public SubTaskComposite() => CompositeType = nameof(SubTaskComposite);
    }
}