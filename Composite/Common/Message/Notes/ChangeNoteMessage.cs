using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes
{
    public class ChangeNoteMessage(NoteBaseVM note)
    {
        public NoteBaseVM Note { get; set; } = note;
    }
}