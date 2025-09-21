using Composite.Models.Notes;

namespace Composite.Repositories
{
    public interface ICategoryNoteRepository
    {
        IEnumerable<CategoryNote> Read();
        Task<bool> Create(CategoryNote categoryNote);
        Task<bool> Delete(string nameCategory);
        Task<bool> SetCategory(string nameCategory);
    }
}