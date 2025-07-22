using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public class NoteService(INoteMap noteMap, INoteRepository noteRepository) : INoteService
    {
        public async Task<bool> AddNoteAsync(NoteVM noteVM)
        {
            noteVM.DateCreate = DateTime.Now;
            var note = noteMap.MapToModel(noteVM);

            if(await noteRepository.Create(note)) return true;

            return false;
        }
    }
}