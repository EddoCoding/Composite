using System.Collections.ObjectModel;
using Composite.ViewModels;

namespace Composite.Services
{
    public interface IMediaPlayerService : IDisposable 
    {
        ObservableCollection<SongVM> Songs { get; set; }
    }
}