using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.ViewModels.Notes;

namespace Composite.Services
{
    public class NoteService(INoteMap noteMap, INoteRepository noteRepository, INoteFactory noteFactory) : INoteService
    {
        public async Task<bool> AddNoteAsync(NoteVM noteVM)
        {
            noteVM.DateCreate = DateTime.Now;
            var note = noteMap.MapToModel(noteVM);

            if(await noteRepository.Create(note)) return true;

            return false;
        }
        public IEnumerable<NoteVM> GetNotes()
        {
            var notes = noteRepository.Read();

            List<NoteVM> notesVM = new();
            foreach (var note in notes)
            {
                var noteVM = noteMap.MapToViewModel(note);
                notesVM.Add(noteVM);
            }
            return notesVM;
        }
        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            if(await noteRepository.Delete(id.ToString())) return true;
            return false;
        }
        public async Task<bool> UpdateNoteAsync(NoteVM noteVM)
        {
            var note = noteMap.MapToModel(noteVM);

            if (await noteRepository.Update(note)) return true;
            return false;
        }

        public async Task<NoteVM> DuplicateNoteVM(NoteVM noteVM)
        {
            var note = noteMap.MapToModel(noteVM);

            var id = Guid.NewGuid();
            var dateCreate = DateTime.Now;

            note.Id = id.ToString();
            note.DateCreate = dateCreate;

            if (await noteRepository.Create(note))
            {
                var duplicateNoteVM = noteMap.MapToViewModel(note);
                duplicateNoteVM.Id = id;

                return duplicateNoteVM;
            }

            return null;
        }
        public NoteVM CreateNoteVM(NoteVM noteVM) => noteFactory.CreateNoteVM(noteVM); 
    }
}