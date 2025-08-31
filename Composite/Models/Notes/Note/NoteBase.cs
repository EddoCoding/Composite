namespace Composite.Models.Notes.Note
{
    public abstract class NoteBase
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}