using Composite.Models.Notes.HardNote;
using Composite.Models.Notes.Note;

namespace Composite.Repositories
{
    public interface IHardNoteRepository
    {
        Task<bool> Create(HardNote hardNote);
        Task<bool> Update(HardNote hardNote);
        Task<bool> Delete(string id);
        IEnumerable<HardNote> Read();
        IEnumerable<HardNote> GetIdTitleNotes();
        Task<HardNote> GetNoteById(string id);
    }
}