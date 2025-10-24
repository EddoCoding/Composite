using Composite.Models.Notes.Note;
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
            Category = noteVM.Category,
            DateCreate = noteVM.DateCreate,
            FontFamily = noteVM.FontFamily,
            FontSize = noteVM.FontSize,
            Password = noteVM.Password
        };
        public NoteVM MapToViewModel(Note note) => new NoteVM()
        {
            Id = Guid.Parse(note.Id),
            Title = note.Title,
            Content = note.Content,
            Category = note.Category,
            DateCreate = note.DateCreate,
            FontFamily = note.FontFamily,
            FontSize = note.FontSize,
            Password = note.Password
        };
    }
}