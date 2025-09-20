using Composite.Models.Notes;

namespace Composite.Repositories
{
    public interface ICategoryNoteRepository
    {
        IEnumerable<CategoryNote> Read();
    }
}