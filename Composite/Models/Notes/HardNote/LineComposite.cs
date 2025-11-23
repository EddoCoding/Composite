namespace Composite.Models.Notes.HardNote
{
    public class LineComposite : CompositeBase
    {
        public int LineSize { get; set; }
        public string LineColor { get; set; } = string.Empty;
        public LineComposite() => CompositeType = nameof(LineComposite);
    }
}
