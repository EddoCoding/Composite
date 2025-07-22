using Composite.Models;
using Composite.ViewModels.Notes;

namespace Composite.Common.Mappers
{
    public interface INoteMap
    {
        Note MapToModel(NoteVM noteVM);
        NoteVM MapToViewModel(Note note);
    }
}