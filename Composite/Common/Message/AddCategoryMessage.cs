using Composite.ViewModels.Notes;

namespace Composite.Common.Message
{
    public class AddCategoryMessage(string nameCategory)
    {
        public CategoryNoteVM CategoryNoteVM { get; set; } = new CategoryNoteVM() { NameCategory = nameCategory };
    }
}