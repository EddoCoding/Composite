using Composite.Models.Notes.Note;
using Composite.ViewModels.Notes.Note;

namespace Composite.Common.Mappers
{
    public interface ICategoryNoteMap
    {
        CategoryNote MapToModel(CategoryNoteVM categoryNoteVM);
        CategoryNoteVM MapToViewModel(CategoryNote categoryNote);
    }
}