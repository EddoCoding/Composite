namespace Composite.ViewModels.Notes.HardNote
{
    public partial class LineCompositeVM : CompositeBaseVM
    {
        public LineCompositeVM() => Id = Guid.NewGuid();
        public override object Clone() => new LineCompositeVM() { Id = Guid.NewGuid() };
    }
}