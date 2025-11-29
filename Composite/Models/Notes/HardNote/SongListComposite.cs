namespace Composite.Models.Notes.HardNote
{
    public class SongListComposite : CompositeBase
    {
        public string Text { get; set; } = string.Empty;

        public SongListComposite()
        {
            CompositeType = nameof(SongListComposite);
            Children = new List<CompositeBase>();
        }
    }
}