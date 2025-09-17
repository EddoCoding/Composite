using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes.Note
{
    public class ChangeNoteBackMessage(NoteBaseVM note)
    {
        public NoteBaseVM Note { get; set; } = note;
    }
}