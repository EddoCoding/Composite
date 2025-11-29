using System.IO;
using Composite.Common.Mappers;
using Composite.Models;
using Composite.Repositories;
using Composite.ViewModels;
using Microsoft.Win32;

namespace Composite.Services
{
    public class SettingMediaPlayerService(ISongMap songMap, ISettingMediaPlayerRepository settingMediaPlayerRepository) : ISettingMediaPlayerService
    {
        public async Task<bool> AddSongs(IEnumerable<Song> songs) => songs?.Any() == true && await settingMediaPlayerRepository.Create(songs);
        public IEnumerable<SongVM> GetSongsVM()
        {
            var songs = settingMediaPlayerRepository.ReadMetaData();

            if (songs != null)
            {
                List<SongVM> songsVM = new();
                foreach(var song in songs)
                {
                    var songVM = songMap.MapToViewModel(song);
                    songsVM.Add(songVM);
                }
                return songsVM;
            }
            return Enumerable.Empty<SongVM>();
        }
        public async Task<byte[]> GetArrayBytesById(Guid id)
        {
            var idString = id.ToString();
            var arrayBytes = await settingMediaPlayerRepository.ReadArrayBytes(idString);
            return arrayBytes;
        }
        public async Task<bool> DeleteSong(Guid id)
        {
            if(await settingMediaPlayerRepository.Delete(id.ToString())) return true;
            return false;
        }
        public async Task DeleteSongs() => await settingMediaPlayerRepository.DeleteAll();
        public async Task<IEnumerable<SongVM>> SelectSongs()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 files|*.mp3",
                Title = "Select songs",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true) return Enumerable.Empty<SongVM>();

            try
            {
                var songs = new List<Song>();
                var idList = new List<string>();

                foreach (string file in openFileDialog.FileNames)
                {
                    if (!File.Exists(file)) continue;

                    var song = new Song
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = Path.GetFileNameWithoutExtension(file),
                        Data = await File.ReadAllBytesAsync(file)
                    };
                    songs.Add(song);
                    idList.Add(song.Id);
                }
                await AddSongs(songs);

                foreach (var s in songs) s.Data = null;
                songs.Clear();
                songs = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var metaDataModels = await settingMediaPlayerRepository.ReadMetaDataAsync(idList);
                List<SongVM> metaDataViewModels = new List<SongVM>();
                foreach(var dataModel in metaDataModels)
                {
                    var metaDataViewModel = songMap.MapToViewModelWithoutData(dataModel);
                    metaDataViewModels.Add(metaDataViewModel);
                }

                return metaDataViewModels;
            }
            catch (Exception ex) { return Enumerable.Empty<SongVM>(); }
        }
    }
}