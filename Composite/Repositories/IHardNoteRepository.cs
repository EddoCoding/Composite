using Composite.Models.Notes.HardNote;

namespace Composite.Repositories
{
    public interface IHardNoteRepository
    {
        Task<bool> Create(HardNote hardNote);
        IEnumerable<HardNote> Read();
        Task<bool> Update(HardNote hardNote);
        Task<bool> Delete(string id);
    }
}