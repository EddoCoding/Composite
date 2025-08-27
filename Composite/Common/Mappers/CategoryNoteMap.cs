using Composite.Models;
using Composite.ViewModels.Notes.Note;

namespace Composite.Common.Mappers
{
    public class CategoryNoteMap : ICategoryNoteMap
    {
        public CategoryNote MapToModel(CategoryNoteVM categoryNoteVM) => new CategoryNote() { NameCategory = categoryNoteVM.NameCategory };
        public CategoryNoteVM MapToViewModel(CategoryNote categoryNote) => new CategoryNoteVM() { NameCategory = categoryNote.NameCategory };
    }
}