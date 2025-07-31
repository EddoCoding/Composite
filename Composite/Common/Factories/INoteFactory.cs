using Composite.ViewModels.Notes;

namespace Composite.Common.Factories
{
    public interface INoteFactory
    {
        NoteVM CreateNoteVM(NoteVM noteVM);
    }
}