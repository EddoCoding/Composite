namespace Composite.Models
{
    public class Note : NoteBase
    {
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
        public string Password { get; set; } = string.Empty;
        public int Preview { get; set; }
    }
}