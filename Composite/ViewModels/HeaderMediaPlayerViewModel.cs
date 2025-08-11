using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace Composite.ViewModels
{
    public partial class HeaderMediaPlayerViewModel : ObservableObject, IDisposable
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;

        MediaPlayer _mediaPlayer;
        List<SongVM> _songs = new();
        bool _isPlaying;
        DispatcherTimer _positionTimer;
        bool _isUserSeeking;

        [ObservableProperty] int _currentSongIndex;
        [ObservableProperty] string _nameSong;
        [ObservableProperty] double _volume = 0.5;
        [ObservableProperty] double _position;
        [ObservableProperty] double _duration;
        [ObservableProperty] string _currentTime = "0:00";
        [ObservableProperty] string _totalTime = "0:00";

        string _tempFilePath;

        public HeaderMediaPlayerViewModel(IViewService viewService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
        {
            _viewService = viewService;
            _messenger = messenger;

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Volume = Volume;

            _songs = new (settingMediaPlayerService.GetSongsVM());

            TimerManagement();

            if (_songs.Any())
            {
                _currentSongIndex = 0;
                UpdateNameSong(_currentSongIndex);

                _mediaPlayer.MediaOpened += (s, e) =>
                {
                    if (_mediaPlayer.NaturalDuration.HasTimeSpan)
                    {
                        Duration = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                        TotalTime = FormatTime(Duration);
                    }
                };
                _mediaPlayer.MediaEnded += (s, e) =>
                {
                    if (_songs.Count == 0) return;
                    _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
                    PlaySong(_currentSongIndex);
                };
            }
            else NameSong = "No songs found";

            messenger.Register<ManagementSongMessage>(this, (r, m) =>
            {
                if (m.Message == "Delete")
                {
                    var songVM = _songs.FirstOrDefault(x => x.Id == m.Id);
                    _songs.Remove(songVM);
                    if (NameSong == songVM.Title) Next();
                    if (_songs.Count == 0)
                    {
                        NameSong = "No songs found";
                        _mediaPlayer.Stop();
                    };
                }
                if (m.Message == "Add")
                {
                    _songs.Add(m.SongVM);
                    if(_songs.Count == 1) NameSong = m.SongVM.Title;
                }
                if (m.Message == "Select")
                {
                    var selectedSongIndex = _songs.FindIndex(x => x.Id == m.SongVM.Id);

                    if (selectedSongIndex >= 0)
                    {
                        _currentSongIndex = selectedSongIndex;
                        PlaySong(_currentSongIndex);
                    }
                    else
                    {
                        _songs.Add(m.SongVM);
                        _currentSongIndex = _songs.Count - 1;
                        PlaySong(_currentSongIndex);
                    }
                }
            });
        }

        [RelayCommand] void Play()
        {
            if (_songs.Count == 0) return;

            if (!_isPlaying)
            {
                if (_mediaPlayer.Source == null) PlaySong(_currentSongIndex);
                else
                {
                    _mediaPlayer.Play();
                    _positionTimer.Start();
                }
            }
            else
            {
                _mediaPlayer.Pause();
                _positionTimer.Stop();
            }
            _isPlaying = !_isPlaying;
        }
        [RelayCommand] void Back()
        {
            if (_songs.Count == 0) return;
            _currentSongIndex = (_currentSongIndex - 1 + _songs.Count) % _songs.Count;
            PlaySong(_currentSongIndex);
        }
        [RelayCommand] void Next()
        {
            if (_songs.Count == 0) return;
            _currentSongIndex = (_currentSongIndex + 1) % _songs.Count;
            PlaySong(_currentSongIndex);
        }

        [RelayCommand] void SetPosition(double newPosition)
        {
            if (_mediaPlayer.Source != null && Duration > 0 && _isUserSeeking)
            {
                var timeSpan = TimeSpan.FromSeconds(newPosition);
                _mediaPlayer.Position = timeSpan;
                CurrentTime = FormatTime(newPosition);
            }
        }
        [RelayCommand] void StartSeeking() => _isUserSeeking = true;
        [RelayCommand] void StopSeeking(object parameter)
        {
            _isUserSeeking = false;
            if (parameter is double finalPosition && _mediaPlayer.Source != null && Duration > 0)
            {
                var timeSpan = TimeSpan.FromSeconds(finalPosition);
                _mediaPlayer.Position = timeSpan;
                Position = finalPosition;
                CurrentTime = FormatTime(finalPosition);
            }
        }
        [RelayCommand] void OpenViewSettingMediaPlayer() => _viewService.ShowView<SettingMediaPlayerViewModel>();

        public void PlaySong(SongVM songvm)
        {
            if (songvm?.Data != null)
            {
                _tempFilePath = Path.GetTempFileName() + ".mp3";
                File.WriteAllBytes(_tempFilePath, songvm.Data);

                _mediaPlayer.Open(new Uri(_tempFilePath));
                _mediaPlayer.Play();
                _isPlaying = true;
                _positionTimer.Start();
            }
        }
        void _mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Close();
            CleanupTempFile();
        }
        void CleanupTempFile()
        {
            if (!string.IsNullOrEmpty(_tempFilePath) && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
                _tempFilePath = null;
            }
        }

        void PlaySong(int index)
        {
            if (index < 0 || index >= _songs.Count) return;

            var song = _songs[index];
            PlaySong(song);
            UpdateNameSong(index);
            Position = 0;
            CurrentTime = "0:00";
        }
        void UpdateNameSong(int index)
        {
            if (index >= 0 && index < _songs.Count) NameSong = _songs[index].Title ?? "Unknown Song";
        }
        partial void OnVolumeChanged(double value) => _mediaPlayer.Volume = value;
        void UpdatePosition(object sender, EventArgs e)
        {
            if (!_isUserSeeking && _mediaPlayer.Source != null)
            {
                if (_mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    Position = _mediaPlayer.Position.TotalSeconds;
                    CurrentTime = FormatTime(Position);
                }
            }
        }
        string FormatTime(double seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
        }
        void TimerManagement()
        {
            _positionTimer = new DispatcherTimer();
            _positionTimer.Interval = TimeSpan.FromMilliseconds(500);
            _positionTimer.Tick += UpdatePosition;
        }

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
                if (disposing)
                {
                    _positionTimer?.Stop();
                    _mediaPlayer?.Stop();
                    _mediaPlayer?.Close();
                    CleanupTempFile();
                    _messenger?.UnregisterAll(this);
                }
                _disposed = true;
            }
        }
    };
}