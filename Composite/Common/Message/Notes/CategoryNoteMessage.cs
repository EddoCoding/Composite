using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes
{
    public class CategoryNoteMessage(CategoryNoteVM categoryNoteVM)
    {
        public CategoryNoteVM CategoryNote { get; set; } = categoryNoteVM;
    }
}