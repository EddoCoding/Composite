namespace Composite.Common.Message
{
    public class PasswordNoteMessage(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}