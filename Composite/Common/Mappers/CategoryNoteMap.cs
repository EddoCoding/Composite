using Composite.Models.Notes;
using Composite.ViewModels.Notes;

namespace Composite.Common.Mappers
{
    public class CategoryNoteMap : ICategoryNoteMap
    {
        public CategoryNote MapToModel(CategoryNoteVM categoryNoteVM) => new CategoryNote() { NameCategory = categoryNoteVM.NameCategory };
        public CategoryNoteVM MapToViewModel(CategoryNote categoryNote) => new CategoryNoteVM() { NameCategory = categoryNote.NameCategory };
    }
}