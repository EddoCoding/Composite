using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;
using System.Collections.ObjectModel;

namespace Composite.ViewModels
{
    public partial class SettingMediaPlayerViewModel : ObservableObject, IDisposable
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        readonly ISettingMediaPlayerService _settingMediaPlayerService;

        public string Title { get; set; } = "Настройка медиаплеера";
        public ObservableCollection<SongVM> Songs { get; set; }

        public SettingMediaPlayerViewModel(IViewService viewService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
        {
            _viewService = viewService;
            _messenger = messenger;
            _settingMediaPlayerService = settingMediaPlayerService;

            Songs = new(settingMediaPlayerService.GetSongsVM());
        }

        [RelayCommand] void Select(SongVM songVM)
        {
            _messenger.Send(new ManagementSongMessage("Select", null, songVM));
            Close();
        }
        [RelayCommand] async void SelectSongs()
        {
            foreach(var songVM in await _settingMediaPlayerService.SelectSongs())
            {
                Songs.Add(songVM);
                _messenger.Send(new ManagementSongMessage("Add", null, songVM));
            }
        }
        [RelayCommand] async void DeleteSong(SongVM song)
        {
            if (await _settingMediaPlayerService.DeleteSong(song.Id))
            {
                Songs.Remove(song);
                _messenger.Send(new ManagementSongMessage("Delete", song.Id));
            }
        }
        [RelayCommand] void Close() => _viewService.CloseView<SettingMediaPlayerViewModel>();

        bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) Songs.Clear();
                _disposed = true;
            }
        }
    }
}