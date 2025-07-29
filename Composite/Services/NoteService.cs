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
        public IEnumerable<NoteVM> GetNotes()
        {
            var notes = noteRepository.Read();

            List<NoteVM> notesVM = new();

            foreach (var note in notes)
            {
                var noteVM = new NoteVM()
                {
                    Id = Guid.Parse(note.Id),
                    Title = note.Title,
                    Content = note.Content,
                    DateCreate = DateTime.Now,
                    Password = note.Password,
                    Preview = note.Preview == 1
                };
                notesVM.Add(noteVM);
            }

            return notesVM;
        }
        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            if(await noteRepository.Delete(id.ToString())) return true;

            return false;
        }

        public async Task<NoteVM> DuplicateNoteVM(NoteVM noteVM)
        {
            var note = noteMap.MapToModel(noteVM);

            var id = Guid.NewGuid();
            var dateCreate = DateTime.Now;

            note.Id = id.ToString();
            note.DateCreate = dateCreate;

            if (await noteRepository.Create(note)) return new NoteVM()
            {
                Id = id,
                Title = note.Title,
                Content = note.Content,
                DateCreate = note.DateCreate,
                Password = note.Password,
                Preview = note.Preview == 1
            };

            return null;
        }
    }
}