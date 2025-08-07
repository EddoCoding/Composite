using Composite.Models;
using Composite.ViewModels.Notes;

namespace Composite.Common.Mappers
{
    public interface ICategoryNoteMap
    {
        CategoryNote MapToModel(CategoryNoteVM categoryNoteVM);
        CategoryNoteVM MapToViewModel(CategoryNote categoryNote);
    }
}