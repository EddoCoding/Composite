using Composite.Common.Mappers;
using Composite.Models;
using Composite.Repositories;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public class HardNoteService(IHardNoteRepository hardNoteRepository, IHardNoteMap hardNoteMap) : IHardNoteService
    {
        public async Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = hardNoteMap.MapToModel(hardNoteVM);

            if (await hardNoteRepository.Create(hardNote)) return true;

            return false;
        }
        public async Task<bool> DeleteHardNoteAsync(Guid id)
        {
            var q = hardNoteRepository.Delete(id.ToString());

            return false;
        }
        public async Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var q = hardNoteRepository.Update(new HardNote());

            return false;
        }
        public IEnumerable<HardNoteVM> GetNotes()
        {
            var q = hardNoteRepository.Read();

            return Enumerable.Empty<HardNoteVM>();
        }
    }
}