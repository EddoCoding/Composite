namespace Composite.Models.Notes.HardNote
{
    public class HardNote
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<CompositeBase> Composites { get; set; }
    }
}