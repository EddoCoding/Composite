namespace Composite.Models.Notes.HardNote
{
    public class QuoteComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public QuoteComposite() => CompositeType = nameof(QuoteComposite);
    }
}
