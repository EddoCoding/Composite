using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface INoteService
    {
        Task<bool> AddNoteAsync(NoteVM noteVM);
        IEnumerable<NoteVM> GetNotes();
        Task<bool> DeleteNoteAsync(Guid id);
        Task<bool> UpdateNoteAsync(NoteVM noteVM);
        IEnumerable<NoteIdTitle> GetIdTitleNotes();
        Task<NoteVM> GetNoteById(Guid id);

        Task<NoteVM> DuplicateNoteVM(NoteVM noteVM);
        NoteVM CreateNoteVM(NoteVM noteVM);
    }
}