namespace Composite.Models
{
    public class TextComposite : CompositeBase
    {
        //public string Text { get; set; } = string.Empty;

        public TextComposite()
        {
            CompositeType = nameof(TextComposite);
        }
    }
}