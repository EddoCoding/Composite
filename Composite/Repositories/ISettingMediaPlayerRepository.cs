using Composite.Models;

namespace Composite.Repositories
{
    public interface ISettingMediaPlayerRepository
    {
        Task<bool> Create(IEnumerable<Song> songs);
        IEnumerable<Song> ReadSong();
        Task<bool> Delete(string id);
    }
}