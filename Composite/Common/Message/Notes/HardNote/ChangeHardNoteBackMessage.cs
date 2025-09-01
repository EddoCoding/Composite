using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Message.Notes.HardNote
{
    public class ChangeHardNoteBackMessage(HardNoteVM hardNoteVM)
    {
        public HardNoteVM HardNoteVM { get; set; } = hardNoteVM;
    }
}