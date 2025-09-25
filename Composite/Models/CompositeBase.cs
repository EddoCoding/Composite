namespace Composite.Models
{
    public class CompositeBase
    {
        public string Id { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string Header { get; set; } = string.Empty;
        public string FontWeightHeader { get; set; } = string.Empty;
        public double FontSizeHeader { get; set; }

        public string Quote { get; set; } = string.Empty;

        public string TaskText { get; set; } = string.Empty;
        public int Completed { get; set; }

        public string HardNoteId { get; set; } = string.Empty;
        public string CompositeType { get; set; } = string.Empty;
    }
}