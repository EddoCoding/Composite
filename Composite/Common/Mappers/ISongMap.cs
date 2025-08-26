using Composite.Models;
using Composite.ViewModels;

namespace Composite.Common.Mappers
{
    public interface ISongMap
    {
        Song MapToModel(SongVM songVM);
        SongVM MapToViewModel(Song song);
        SongVM MapToViewModelWithoutData(Song song);
    }
}