using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public class HardNoteService(IHardNoteRepository hardNoteRepository, IHardNoteMap hardNoteMap, IHardNoteFactory hardNoteFactory) : IHardNoteService
    {
        public async Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = hardNoteMap.MapToModel(hardNoteVM);

            if (await hardNoteRepository.Create(hardNote)) return true;

            return false;
        }
        public async Task<bool> DeleteHardNoteAsync(Guid id)
        {
            if(await hardNoteRepository.Delete(id.ToString())) return true;

            return false;
        }
        public async Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = hardNoteMap.MapToModel(hardNoteVM);

            if (await hardNoteRepository.Update(hardNote)) return true;

            return false;
        }
        public IEnumerable<HardNoteVM> GetNotes()
        {
            var hardNotes = hardNoteRepository.Read();

            List<HardNoteVM> hardNotesVM = new();
            foreach (var hardNote in hardNotes)
            {
                var hardNoteVM = hardNoteMap.MapToViewModel(hardNote);
                hardNotesVM.Add(hardNoteVM);
            }
            return hardNotesVM;
        }

        public async Task<HardNoteVM> DuplicateHardNoteVM(NoteBaseVM hardNoteVM)
        {
            var id = Guid.NewGuid();
            hardNoteVM.Id = id;
            var note = hardNoteMap.MapToModelWithNewIdComposite((HardNoteVM)hardNoteVM);

            if (await hardNoteRepository.Create(note))
            {
                var duplicateHardNoteVM = hardNoteMap.MapToViewModel(note);
                duplicateHardNoteVM.Id = id;

                return duplicateHardNoteVM;
            }
            return null;
        }
        public HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM) => hardNoteFactory.CreateHardNoteVM(hardNoteVM);
    }
}