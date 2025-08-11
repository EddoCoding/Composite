namespace Composite.ViewModels
{
    public class SongVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public byte[] Data { get; set; }

        public SongVM() => Id = Guid.NewGuid();
    }
}