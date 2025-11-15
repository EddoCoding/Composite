namespace Composite.Models
{
    public class CompositeBase
    {
        public string Id { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public string FontWeightHeader { get; set; } = string.Empty;
        public double FontSizeHeader { get; set; }

        public int Completed { get; set; }

        public byte[] Data { get; set; }
        public string HorizontalImage { get; set; } = string.Empty;

        public string ValueRef { get; set; } = string.Empty;

        public int Number { get; set; }

        public int BrSize { get; set; }
        public int BrCornerRadius { get; set; }
        public string BrColor { get; set; }
        public string BgColor { get; set; }

        public string HardNoteId { get; set; } = string.Empty;
        public string CompositeType { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}