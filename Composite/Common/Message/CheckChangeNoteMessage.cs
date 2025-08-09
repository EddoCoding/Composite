namespace Composite.Common.Message
{
    public class CheckChangeNoteMessage(Guid id, Guid idNote, string titleNote)
    {
        public Guid Id { get; set; } = id;
        public Guid IdNote { get; set; } = idNote;
        public string TitleNote { get; set; } = titleNote;
    }
}
