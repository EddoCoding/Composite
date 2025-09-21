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

            try
            {
                if (await hardNoteRepository.Create(hardNote)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteHardNoteAsync(Guid id)
        {
            try
            {
                if (await hardNoteRepository.Delete(id.ToString())) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = hardNoteMap.MapToModel(hardNoteVM);

            try
            {
                if (await hardNoteRepository.Update(hardNote)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<HardNoteVM> GetNotes()
        {
            List<HardNoteVM> hardNotesVM = new();

            try
            {
                var hardNotes = hardNoteRepository.Read();
                foreach (var hardNote in hardNotes)
                {
                    var hardNoteVM = hardNoteMap.MapToViewModel(hardNote);
                    hardNotesVM.Add(hardNoteVM);
                }
                return hardNotesVM;
            }
            catch (Exception)
            {
                return hardNotesVM;
            }
        }

        public async Task<HardNoteVM> DuplicateHardNoteVM(NoteBaseVM hardNoteVM)
        {
            var currentId = hardNoteVM.Id;
            var id = Guid.NewGuid();
            hardNoteVM.Id = id;
            var note = hardNoteMap.MapToModelWithNewIdComposite((HardNoteVM)hardNoteVM);
            note.Title = note.Title + " (duplicate)";

            try
            {
                if (await hardNoteRepository.Create(note))
                {
                    hardNoteVM.Id = currentId;
                    var duplicateHardNoteVM = hardNoteMap.MapToViewModel(note);

                    return duplicateHardNoteVM;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM) => hardNoteFactory.CreateHardNoteVM(hardNoteVM);
    }
}