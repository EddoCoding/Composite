using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public interface INoteService
    {
        Task<bool> AddNoteAsync(NoteVM noteVM);
    }
}