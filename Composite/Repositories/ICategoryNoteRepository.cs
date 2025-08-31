using Composite.Models.Notes.Note;

namespace Composite.Repositories
{
    public interface ICategoryNoteRepository
    {
        Task<bool> Create(CategoryNote categoryNote);
        IEnumerable<CategoryNote> Read();
        Task<bool> Delete(string NameCategory);
    }
}