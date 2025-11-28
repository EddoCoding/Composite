namespace Composite.Models.Notes.HardNote
{
    public class DocumentComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public byte[] Data { get; set; }

        public DocumentComposite() => CompositeType = nameof(DocumentComposite);
    }
}