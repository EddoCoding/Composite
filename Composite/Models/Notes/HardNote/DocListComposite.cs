namespace Composite.Models.Notes.HardNote
{
    public class DocListComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;

        public DocListComposite()
        {
            CompositeType = nameof(DocListComposite);
            Children = new List<CompositeBase>();
        }
    }
}