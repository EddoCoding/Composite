namespace Composite.Services
{
    public interface ISettingMediaPlayerService
    {
        Task<bool> InsertUpdatePath(string path);
        string GetPath();

        string SelectPathFolder();
    }
}