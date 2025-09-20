namespace Composite.Models.Notes.Note
{
    public class Note : NoteBase
    {
        public string Content { get; set; }
        public string FontFamily { get; set; } = string.Empty;
        public double FontSize { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; }
    }
}