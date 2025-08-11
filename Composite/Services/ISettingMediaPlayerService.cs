using Composite.Models;
using Composite.ViewModels;

namespace Composite.Services
{
    public interface ISettingMediaPlayerService
    {
        Task<bool> AddSongs(IEnumerable<Song> songs);
        IEnumerable<SongVM> GetSongsVM();
        Task<bool> DeleteSong(Guid id);

        Task<IEnumerable<SongVM>> SelectSongs();
    }
}