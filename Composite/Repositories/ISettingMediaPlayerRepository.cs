using Composite.Models;

namespace Composite.Repositories
{
    public interface ISettingMediaPlayerRepository
    {
        Task<bool> Create(IEnumerable<Song> songs);
        IEnumerable<Song> ReadMetaData();
        Task<IEnumerable<Song>> ReadMetaDataAsync(List<string> idList);
        Task<byte[]> ReadArrayBytes(string id);
        Task<bool> Delete(string id);
        Task DeleteAll();
    }
}