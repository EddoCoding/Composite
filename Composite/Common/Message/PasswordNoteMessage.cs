namespace Composite.Common.Message
{
    public class PasswordNoteMessage(string password)
    {
        public string Password { get; set; } = password;
    }
}