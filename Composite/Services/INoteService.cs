using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface INoteService
    {
        Task<bool> AddNoteAsync(NoteVM noteVM);
        IEnumerable<NoteVM> GetNotes();
        Task<bool> DeleteNoteAsync(Guid id);
        Task<NoteVM> DuplicateNoteVM(NoteVM noteVM);
    }
}