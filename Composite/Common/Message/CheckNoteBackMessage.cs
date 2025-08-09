namespace Composite.Common.Message
{
    public class CheckNoteBackMessage(Guid id, bool titleNote, string error = "")
    {
        public Guid Id { get; set; } = id;
        public bool TitleNote { get; set; } = titleNote;
        public string ErrorMessage { get; set; } = error;
    }
}