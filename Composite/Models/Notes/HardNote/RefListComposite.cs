namespace Composite.Models.Notes.HardNote
{
    public class RefListComposite : CompositeBase
    {
        public RefListComposite()
        {
            CompositeType = nameof(RefListComposite);
            Children = new List<CompositeBase>();
        }
    }
}