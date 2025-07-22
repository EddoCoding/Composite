using Composite.Models;

namespace Composite.Repositories
{
    public interface INoteRepository
    {
        Task<bool> Create(Note note);
    }
}