namespace Composite.Common.Message.Notes.Note
{
    public class PasswordNoteBackMessage(Guid id, string password)
    {
        public Guid Id { get; set; } = id;
        public string Password { get; set; } = password;
    }
}
