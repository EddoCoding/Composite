namespace Composite.Models
{
    public class CompositeBase
    {
        public string Id { get; set; }
        public string Tag { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string? Header { get; set; }
        public string? Text { get; set; }
        public string HardNoteId { get; set; } = string.Empty;
        public string CompositeType { get; set; } = string.Empty;
    }
}