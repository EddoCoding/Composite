using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Message.Notes.HardNote
{
    public class AddHardNoteMessage(HardNoteVM hardNoteVM)
    {
        public HardNoteVM HardNoteVM { get; set; } = hardNoteVM;
    }
}