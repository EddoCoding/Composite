using Composite.ViewModels.Notes;

namespace Composite.Common.Message.Notes
{
    public class DeleteCategoryNoteMessage(CategoryNoteVM category)
    {
        public CategoryNoteVM Category { get; set; } = category;
    }
}