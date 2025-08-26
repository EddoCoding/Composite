namespace Composite.ViewModels
{
    public class SongVM
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public byte[] Data { get; set; }
    }
}