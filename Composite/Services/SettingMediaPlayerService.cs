using Composite.Repositories;
using Dapper;
using Microsoft.Win32;

namespace Composite.Services
{
    public class SettingMediaPlayerService(ISettingMediaPlayerRepository settingMediaPlayerRepository) : ISettingMediaPlayerService
    {
       
        public async Task<bool> InsertUpdatePath(string path)
        {
            if(await settingMediaPlayerRepository.Create(path)) return true;
            return false;
        }
        public string GetPath() => settingMediaPlayerRepository.Read();

        public string SelectPathFolder()
        {
            OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Выбор пути";
            bool? result = dialog.ShowDialog();
            return dialog.FolderName;
        }
    }
}