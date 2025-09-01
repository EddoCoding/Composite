namespace Composite.Common.Message.Notes.Note
{
    public class InputPasswordDeleteMessage(Guid id, string password)
    {
        public Guid Id { get; set; } = id;
        public string Password { get; set; } = password;
    }
}