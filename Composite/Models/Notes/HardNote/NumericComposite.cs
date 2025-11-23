namespace Composite.Models.Notes.HardNote
{
    public class NumericComposite : CompositeBase
    {
        public int Number { get; set; }
        public string Text { get; set; } = string.Empty;
        public NumericComposite() => CompositeType = nameof(NumericComposite);
    }
}