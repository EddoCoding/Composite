namespace Composite.Models
{
    public class HardNote
    {
        public string Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<CompositeBase> Composites { get; set; }
    }
}