namespace Composite.Models
{
    public abstract class NoteBase
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}