namespace Composite.Common.Message.Notes.Note
{
    public class InputPasswordDeleteBackMessage(Guid id)
    {
        public Guid Id { get; set; } = id;
    }
}