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
            try
            {
                if (await noteRepository.Create(note)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<NoteVM> GetNotes()
        {
            var notes = noteRepository.Read();
            List<NoteVM> notesVM = new();

            try
            {
                foreach (var note in notes)
                {
                    var noteVM = noteMap.MapToViewModel(note);
                    notesVM.Add(noteVM);
                }
                return notesVM;
            }
            catch (Exception)
            {
                return notesVM;
            }
        }
        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            try
            {
                if (await noteRepository.Delete(id.ToString())) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateNoteAsync(NoteVM noteVM)
        {
            var note = noteMap.MapToModel(noteVM);

            try
            {
                if (await noteRepository.Update(note)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<NoteIdTitle> GetIdTitleNotes()
        {
            var notes = noteRepository.GetIdTitleNotes();
            List<NoteIdTitle> notesIdTitle = new();

            try
            {
                foreach (var note in notes)
                {
                    var noteIdTitle = noteMap.MapToNoteIdTitle(note);
                    notesIdTitle.Add(noteIdTitle);
                }
                return notesIdTitle;
            }
            catch (Exception)
            {
                return notesIdTitle;
            }
        }
        public async Task<NoteVM> GetNoteById(Guid id)
        {
            var note = await noteRepository.GetNoteById(id.ToString());
            if (note == null) return null;

            try
            {
                var noteVM = noteMap.MapToViewModel(note);
                return noteVM;
            }
            catch (Exception) 
            {
                return null;
            };
        }

        public async Task<NoteVM> DuplicateNoteVM(NoteVM noteVM)
        {
            var id = Guid.NewGuid();
            var note = noteMap.MapToModel(noteVM);
            note.Id = id.ToString();
            note.Title = note.Title + " (duplicate)";
            note.DateCreate = DateTime.Now;

            try
            {
                if (await noteRepository.Create(note))
                {
                    var duplicateNoteVM = noteMap.MapToViewModel(note);

                    return duplicateNoteVM;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public NoteVM CreateNoteVM(NoteVM noteVM) => noteFactory.CreateNoteVM(noteVM);
    }
}