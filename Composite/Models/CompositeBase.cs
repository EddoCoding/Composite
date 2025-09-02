namespace Composite.Models
{
    public class CompositeBase
    {
        public string Id { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string? Text { get; set; } = string.Empty;
        public string HardNoteId { get; set; } = string.Empty;
        public string CompositeType { get; set; } = string.Empty;
    }
}