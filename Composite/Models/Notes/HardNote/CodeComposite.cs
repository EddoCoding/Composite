namespace Composite.Models.Notes.HardNote
{
    public class CodeComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public CodeComposite() => CompositeType = nameof(CodeComposite);
    }
}