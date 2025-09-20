using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface ICategoryNoteService
    {
        IEnumerable<CategoryNoteVM> GetCategories();
    }
}