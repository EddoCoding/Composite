using Composite.Models;
using Composite.ViewModels;

namespace Composite.Services
{
    public interface ISettingMediaPlayerService
    {
        Task<bool> AddSongs(IEnumerable<Song> songs);
        IEnumerable<SongVM> GetSongsVM();
        Task<byte[]> GetArrayBytesById(Guid id);
        Task<bool> DeleteSong(Guid id);
        Task DeleteSongs();

        Task<IEnumerable<SongVM>> SelectSongs();
    }
}