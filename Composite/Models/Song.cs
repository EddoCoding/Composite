namespace Composite.Models
{
    public class Song
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public byte[] Data { get; set; }
    }
}