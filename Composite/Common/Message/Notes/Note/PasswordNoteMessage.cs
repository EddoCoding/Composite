namespace Composite.Common.Message.Notes.Note
{
    public class PasswordNoteMessage(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}