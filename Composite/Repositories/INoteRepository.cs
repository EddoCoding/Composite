using Composite.Models.Notes.Note;

namespace Composite.Repositories
{
    public interface INoteRepository
    {
        Task<bool> Create(Note note);
        IEnumerable<Note> Read();
        Task<bool> Update(Note note);
        Task<bool> Delete(string id);
        IEnumerable<Note> GetIdTitleNotes();
        Task<Note> GetNoteById(string id);
    }
}