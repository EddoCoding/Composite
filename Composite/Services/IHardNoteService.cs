using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public interface IHardNoteService
    {
        Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM);
        Task<bool> DeleteHardNoteAsync(Guid id);
        Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM);
        IEnumerable<HardNoteVM> GetNotes();
        IEnumerable<NoteIdTitle> GetIdTitleNotes();
        Task<HardNoteVM> GetNoteById(Guid id);

        Task<HardNoteVM> DuplicateHardNoteVM(NoteBaseVM hardNoteVM);
        HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM);
    }
}