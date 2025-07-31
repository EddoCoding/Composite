using Composite.ViewModels.Notes;

namespace Composite.Common.Message
{
    public class ChangeNoteMessage(NoteVM noteVM)
    {
        public NoteVM NoteVM { get; set; } = noteVM;
    }
}