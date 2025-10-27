using Composite.ViewModels.Notes.HardNote;

namespace Composite.Services
{
    public interface INoteCommonService
    {
        Task CheckValuRef(RefCompositeVM refComposite);
    }
}