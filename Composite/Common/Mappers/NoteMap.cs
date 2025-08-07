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
            Preview = noteVM.Preview ? 1 : 0,
            FontFamily = noteVM.FontFamily,
            FontSize = noteVM.FontSize,
            Category = noteVM.Category,
            Color = noteVM.Color
        };
        public NoteVM MapToViewModel(Note note) => new NoteVM()
        {
            Id = Guid.Parse(note.Id),
            Title = note.Title,
            Content = note.Content,
            DateCreate = note.DateCreate,
            Password = note.Password,
            Preview = note.Preview == 1,
            FontFamily = note.FontFamily,
            FontSize = note.FontSize,
            Category = note.Category,
            Color = note.Color
        };
    }
}