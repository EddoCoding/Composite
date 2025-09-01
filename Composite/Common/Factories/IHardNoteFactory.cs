using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Factories
{
    public interface IHardNoteFactory
    {
        HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM);
    }
}