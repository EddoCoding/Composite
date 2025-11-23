namespace Composite.Models.Notes.HardNote
{
    public class DocComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public byte[] Data { get; set; }
        public DocComposite() => CompositeType = nameof(DocComposite);
    }
}