using Composite.Models;
using Composite.ViewModels.Notes;

namespace Composite.Common.Mappers
{
    public class NoteMap : INoteMap
    {
        public Note MapToModel(NoteVM noteVM) => new Note()
        {
            Id = noteVM.Id.ToString(),
            Title = noteVM.Title,
            Content = noteVM.Content,
            DateCreate = noteVM.DateCreate,
            Password = noteVM.Password,
            Preview = noteVM.Preview ? 1 : 0
        };

        public NoteVM MapToViewModel(Note note) => new NoteVM()
        {
            Id = Guid.Parse(note.Id),
            Title = note.Title,
            Content = note.Content,
            DateCreate = note.DateCreate,
            Password = note.Password,
            Preview = note.Preview == 1
        };
    }
}