using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public class HardNoteService : IHardNoteService
    {
        readonly IHardNoteRepository _hardNoteRepository;
        readonly IHardNoteMap _hardNoteMap;
        readonly IHardNoteFactory _hardNoteFactory;

        public HardNoteService(ITabService tabService, INoteService noteService, IHardNoteRepository hardNoteRepository, IMessenger messenger)
        {
            _hardNoteRepository = hardNoteRepository;
            _hardNoteFactory = new HardNoteFactory(tabService, noteService, this, messenger);
            _hardNoteMap = new HardNoteMap(tabService, noteService, this, messenger);
        }

        public async Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = _hardNoteMap.MapToModel(hardNoteVM);

            try
            {
                if (await _hardNoteRepository.Create(hardNote)) return true;
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
                if (await _hardNoteRepository.Delete(id.ToString())) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = _hardNoteMap.MapToModel(hardNoteVM);

            try
            {
                if (await _hardNoteRepository.Update(hardNote)) return true;
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
                var hardNotes = _hardNoteRepository.Read();
                foreach (var hardNote in hardNotes)
                {
                    var hardNoteVM = _hardNoteMap.MapToViewModel(hardNote);
                    hardNotesVM.Add(hardNoteVM);
                }
                return hardNotesVM;
            }
            catch (Exception)
            {
                return hardNotesVM;
            }
        }
        public IEnumerable<NoteIdTitle> GetIdTitleNotes()
        {
            List<NoteIdTitle> hardNotesIdTitle = new();

            try
            {
                var hardNotes = _hardNoteRepository.GetIdTitleNotes();
                foreach (var hardNote in hardNotes)
                {
                    var hardNoteIdTitle = _hardNoteMap.MapToHardNoteIdTitle(hardNote);
                    hardNotesIdTitle.Add(hardNoteIdTitle);
                }
                return hardNotesIdTitle;
            }
            catch (Exception)
            {
                return hardNotesIdTitle;
            }
        }
        public async Task<HardNoteVM> GetNoteById(Guid id)
        {
            try
            {
                var hardNote = await _hardNoteRepository.GetNoteById(id.ToString());
                if (hardNote == null) return null;

                var hardNoteVM = _hardNoteMap.MapToViewModel(hardNote);
                return hardNoteVM;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HardNoteVM> DuplicateHardNoteVM(NoteBaseVM hardNoteVM)
        {
            var currentId = hardNoteVM.Id;
            var id = Guid.NewGuid();
            hardNoteVM.Id = id;
            var note = _hardNoteMap.MapToModelWithNewIdComposite((HardNoteVM)hardNoteVM);
            note.Title = note.Title + " (duplicate)";

            try
            {
                if (await _hardNoteRepository.Create(note))
                {
                    hardNoteVM.Id = currentId;
                    var duplicateHardNoteVM = _hardNoteMap.MapToViewModel(note);

                    return duplicateHardNoteVM;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM) => _hardNoteFactory.CreateHardNoteVM(hardNoteVM);
    }
}