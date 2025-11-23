namespace Composite.Models.Notes.HardNote
{
    public class FormattedTextComposite : CompositeBase
    {
        public int BorderSize { get; set; }
        public int CornerRadius { get; set; }
        public string BorderColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public byte[] Data { get; set; }
        public FormattedTextComposite() => CompositeType = nameof(FormattedTextComposite);
    }
}