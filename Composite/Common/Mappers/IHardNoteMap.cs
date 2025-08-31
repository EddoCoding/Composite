using Composite.Models;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Mappers
{
    public interface IHardNoteMap
    {
        HardNote MapToModel(HardNoteVM hardNoteVM);
        HardNoteVM MapToViewModel(HardNote hardNote);
    }
}