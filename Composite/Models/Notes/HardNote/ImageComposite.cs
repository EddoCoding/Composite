namespace Composite.Models.Notes.HardNote
{
    public class ImageComposite : CompositeBase
    {
        public string HorizontalAlignment { get; set; } = string.Empty;
        public byte[] Data { get; set; }
        public ImageComposite() => CompositeType = nameof(ImageComposite);
    }
}
