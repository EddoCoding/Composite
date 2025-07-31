namespace Composite.Common.Message
{
    public class InputPasswordMessage(Guid id, string password)
    {
        public Guid Id { get; set; } = id;
        public string Password { get; set; } = password;
    }
}