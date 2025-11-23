namespace Composite.Models.Notes.HardNote
{
    public class HeaderComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public string FontWeight { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public HeaderComposite() => CompositeType = nameof(HeaderComposite);
    }
}