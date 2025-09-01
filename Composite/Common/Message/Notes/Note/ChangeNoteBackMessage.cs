using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes.Note
{
    public class ChangeNoteBackMessage(NoteVM noteVM)
    {
        public NoteVM NoteVM { get; set; } = noteVM;
    }
}