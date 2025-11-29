namespace Composite.Models.Notes.HardNote
{
    public class SongComposite : CompositeBase
    {
        public string Title { get; set; } = string.Empty;
        public byte[] Data { get; set; }

        public SongComposite() => CompositeType = nameof(SongComposite);
    }
}