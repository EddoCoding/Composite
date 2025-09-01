using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes.Note
{
    public class ChangeNoteMessage(NoteVM noteVM)
    {
        public NoteVM NoteVM { get; set; } = noteVM;
    }
}