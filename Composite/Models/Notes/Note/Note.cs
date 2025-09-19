namespace Composite.Models.Notes.Note
{
    public class Note : NoteBase
    {
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public string FontFamily { get; set; } = string.Empty;
        public double FontSize { get; set; }
    }
}