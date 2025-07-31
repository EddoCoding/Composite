using Composite.ViewModels.Notes;

namespace Composite.Common.Message
{
    public class ChangeNoteBackMessage(NoteVM noteVM)
    {
        public NoteVM NoteVM { get; set; } = noteVM;
    }
}