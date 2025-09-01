using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Message.Notes.HardNote
{
    public class ChangeHardNoteMessage(HardNoteVM hardNoteVM)
    {
        public HardNoteVM HardNoteVM { get; set; } = hardNoteVM;
    }
}