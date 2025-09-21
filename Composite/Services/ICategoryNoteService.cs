using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface ICategoryNoteService
    {
        IEnumerable<CategoryNoteVM> GetCategories();
        Task<bool> AddCategory(CategoryNoteVM categoryNoteVM);
        Task<bool> DeleteCategory(string nameCategory);
        Task<bool> SetCategory(string nameCategory);
    }
}