using Composite.Models.Notes.HardNote;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Mappers
{
    public interface IHardNoteMap
    {
        HardNote MapToModel(HardNoteVM hardNoteVM);
        HardNote MapToModelWithNewIdComposite(HardNoteVM hardNoteVM);
        HardNoteVM MapToViewModel(HardNote hardNote);
    }
}