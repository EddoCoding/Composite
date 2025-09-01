namespace Composite.Common.Message.Notes.Note
{
    public class CheckNoteMessage(Guid id, string titleNote)
    {
        public Guid Id { get; set; } = id;
        public string TitleNote { get; set; } = titleNote;
    }
}