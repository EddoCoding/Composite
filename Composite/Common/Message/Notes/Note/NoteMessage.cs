using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes.Note
{
    public class NoteMessage(NoteVM noteVM)
    {
        public NoteVM NoteVM { get; set; } = noteVM;
    }
}