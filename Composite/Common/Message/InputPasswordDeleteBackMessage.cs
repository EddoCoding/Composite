namespace Composite.Common.Message
{
    public class InputPasswordDeleteBackMessage(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}