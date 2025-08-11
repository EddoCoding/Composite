using Composite.Common.Mappers;
using Composite.Models;
using Composite.Repositories;
using Composite.ViewModels;
using Microsoft.Win32;
using System.IO;

namespace Composite.Services
{
    public class SettingMediaPlayerService(ISongMap songMap, ISettingMediaPlayerRepository settingMediaPlayerRepository) : ISettingMediaPlayerService
    {
        public async Task<bool> AddSongs(IEnumerable<Song> songs)
        {
            if (songs != null && await settingMediaPlayerRepository.Create(songs)) return true;
            return false;
        }
        public IEnumerable<SongVM> GetSongsVM()
        {
            var songs = settingMediaPlayerRepository.ReadSong();

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
        public async Task<bool> DeleteSong(Guid id)
        {
            if(await settingMediaPlayerRepository.Delete(id.ToString())) return true;
            return false;
        }

        public async Task<IEnumerable<SongVM>> SelectSongs()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.Title = "Выбор песен";

            List<Song> songs;
            List<SongVM> songsVM;
            if (openFileDialog.ShowDialog() == true)
            {
                songs = new();
                songsVM = new();

                foreach (string file in openFileDialog.FileNames)
                {
                    var song = new Song
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = Path.GetFileNameWithoutExtension(file),
                        Data = File.ReadAllBytes(file)
                    };
                    songs.Add(song);

                    var songVM = songMap.MapToViewModel(song);
                    songsVM.Add(songVM);
                }
                await AddSongs(songs);
                return songsVM;
            }
            return Enumerable.Empty<SongVM>();
        }
    }
}