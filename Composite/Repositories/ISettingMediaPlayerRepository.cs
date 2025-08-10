namespace Composite.Repositories
{
    public interface ISettingMediaPlayerRepository
    {
        Task<bool> Create(string path);
        string Read();
    }
}