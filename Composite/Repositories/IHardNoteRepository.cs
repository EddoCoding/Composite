using Composite.Models.Notes.HardNote;

namespace Composite.Repositories
{
    public interface IHardNoteRepository
    {
        Task<bool> Create(HardNote hardNote);
        Task<bool> Update(HardNote hardNote);
        Task<bool> Delete(string id);
        IEnumerable<HardNote> Read();
    }
}