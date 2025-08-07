using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface ICategoryNoteService
    {
        Task<bool> AddCategory(CategoryNoteVM categoryNoteVM);
        IEnumerable<string> GetCategoryNotes();
        Task<bool> DeleteCategory(string nameCategory);
    }
}