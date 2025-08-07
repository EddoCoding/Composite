namespace Composite.Models
{
    public class Note : NoteBase
    {
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public string Password { get; set; } = string.Empty;
        public int Preview { get; set; }
        public string FontFamily { get; set; } = string.Empty;
        public double FontSize { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}