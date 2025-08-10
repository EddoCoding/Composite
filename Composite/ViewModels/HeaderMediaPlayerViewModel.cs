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
        List<string> _songs = new();
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

        public HeaderMediaPlayerViewModel(IViewService viewService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
        {
            _viewService = viewService;
            _messenger = messenger;

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Volume = Volume;

            var path = settingMediaPlayerService.GetPath();
            if (Directory.Exists(path))
            {
                _songs = Directory.GetFiles(path, "*.mp3").ToList();
                TimerManagement();
            }
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

            messenger.Register<PathFolderMessage>(this, (r, m) => 
            {
                _songs.Clear();
                _positionTimer?.Stop();
                _mediaPlayer?.Close();
                if (Directory.Exists(m.Path))
                {
                    _songs = Directory.GetFiles(m.Path, "*.mp3").ToList();
                    TimerManagement();
                }
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
            });
        }

        [RelayCommand] void Play()
        {
            if (_songs.Count == 0) return;

            if (!_isPlaying)
            {
                if (_mediaPlayer.Source == null) PlaySong(_currentSongIndex);
                else _mediaPlayer.Play();
                _positionTimer.Start();
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

        void PlaySong(int index)
        {
            _mediaPlayer.Open(new Uri(_songs[index]));
            _mediaPlayer.Play();
            _isPlaying = true;
            _positionTimer.Start();
            UpdateNameSong(index);
            Position = 0;
            CurrentTime = "0:00";
        }
        void UpdateNameSong(int index) => NameSong = Path.GetFileNameWithoutExtension(_songs[index]);
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
                    _mediaPlayer?.Close();

                    _messenger.UnregisterAll(this);
                }
                _disposed = true;
            }
        }

        [RelayCommand] void OpenViewSettingMediaPlayer() => _viewService.ShowView<SettingMediaPlayerViewModel>();
    };
}