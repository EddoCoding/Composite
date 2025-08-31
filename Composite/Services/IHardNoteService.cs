using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public interface IHardNoteService
    {
        Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM);
        Task<bool> DeleteHardNoteAsync(Guid id);
        Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM);
        IEnumerable<HardNoteVM> GetNotes();
    }
}