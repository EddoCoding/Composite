using Composite.ViewModels.Notes;

namespace Composite.Common.Factories
{
    public class NoteFactory : INoteFactory
    {
        public NoteVM CreateNoteVM(NoteVM noteVM) => new NoteVM()
        {
            Id = noteVM.Id,
            Title = noteVM.Title,
            Content = noteVM.Content,
            DateCreate = noteVM.DateCreate,
            FontFamily = noteVM.FontFamily,
            FontSize = noteVM.FontSize,
            Category = noteVM.Category,
            Color = noteVM.Color
        };
    }
}