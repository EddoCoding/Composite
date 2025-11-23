namespace Composite.Models.Notes.HardNote
{
    public class MarkerComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public MarkerComposite() => CompositeType = nameof(MarkerComposite);
    }
}
