namespace Composite.Models.Notes.HardNote
{
    public class RefComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;
        public string ValueRef { get; set; } = string.Empty;
        public RefComposite() => CompositeType = nameof(RefComposite);
    }
}