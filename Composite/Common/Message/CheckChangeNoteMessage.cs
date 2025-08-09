namespace Composite.Common.Message
{
    public class CheckChangeNoteMessage(Guid id, string titleNote)
    {
        public Guid Id { get; set; } = id;
        public string TitleNote { get; set; } = titleNote;
    }
}
