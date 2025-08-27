using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.ViewModels;
using NAudio.Wave;

namespace Composite.Services
{
    public partial class MediaPlayerService : ObservableObject, IMediaPlayerService, IDisposable
    {
        readonly ISettingMediaPlayerService _settingMediaPlayerService;

        WaveOutEvent _outputDevice;
        WaveStream _audioReader;
        MemoryStream _currentAudioStream;
        DispatcherTimer _positionTimer;
        bool _isPlaying;
        bool _isUserSeeking;
        bool _isRepeat;

        public ObservableCollection<SongVM> Songs { get; set; }

        [ObservableProperty] int _currentSongIndex;
        [ObservableProperty] string _nameSong;
        [ObservableProperty] double _volume = 0.5;
        [ObservableProperty] double _position;
        [ObservableProperty] double _duration;
        [ObservableProperty] string _currentTime = "0:00";
        [ObservableProperty] string _totalTime = "0:00";
        [ObservableProperty] string _pathImageRepeat = "/Common/Images/notRepeat.png";

        public MediaPlayerService(ISettingMediaPlayerService settingMediaPlayerService)
        {
            _settingMediaPlayerService = settingMediaPlayerService;

            InitializeAudioPlayer();
            Songs = new(settingMediaPlayerService.GetSongsVM());
            TimerManagement();

            if (Songs.Any())
            {
                _currentSongIndex = 0;
                UpdateNameSong(_currentSongIndex);
            }
            else NameSong = "No songs found";
        }

        [RelayCommand] async Task Play()
        {
            if (Songs.Count == 0) return;

            try
            {
                if (!_isPlaying)
                {
                    if (_outputDevice.PlaybackState == PlaybackState.Stopped) await PlaySong(_currentSongIndex);
                    else
                    {
                        _outputDevice.Play();
                        _positionTimer.Start();
                    }
                }
                else
                {
                    _outputDevice.Pause();
                    _positionTimer.Stop();
                }
                _isPlaying = !_isPlaying;
            }
            catch (Exception ex) { _isPlaying = false; }
        }
        [RelayCommand] async Task Next()
        {
            if (Songs.Count == 0) return;

            StopAndCleanCurrentSong();
            _currentSongIndex = (_currentSongIndex + 1) % Songs.Count;
            await PlaySongOptimized(_currentSongIndex);
        }
        [RelayCommand] async Task Back()
        {
            if (Songs.Count == 0) return;

            StopAndCleanCurrentSong();
            _currentSongIndex = (_currentSongIndex - 1 + Songs.Count) % Songs.Count;
            await PlaySongOptimized(_currentSongIndex);
        }
        [RelayCommand] void Repeat()
        {
            if (PathImageRepeat == "/Common/Images/notRepeat.png")
            {
                _isRepeat = true;
                PathImageRepeat = "/Common/Images/repeat.png";
            }
            else
            {
                PathImageRepeat = "/Common/Images/notRepeat.png";
                _isRepeat = false;
            }
        }

        [RelayCommand] async Task SelectSongs()
        {
            var songsVM = await _settingMediaPlayerService.SelectSongs();
            var currentCountSongs = Songs.Count;
            foreach (var songVM in songsVM) Songs.Add(songVM);
            if (Songs.Count > currentCountSongs && currentCountSongs == 0) UpdateNameSong(0);
        }
        [RelayCommand] async Task SelectSong(SongVM songVM)
        {
            if (songVM == null || Songs.Count == 0) return;

            var songIndex = Songs.IndexOf(songVM);
            if (songIndex == -1) return;

            StopAndCleanCurrentSong();

            _currentSongIndex = songIndex;
            await PlaySongOptimized(_currentSongIndex);
        }
        [RelayCommand] async Task DeleteSongs()
        {
            await _settingMediaPlayerService.DeleteSongs();

            if (Songs.Any())
            {
                StopAndCleanCurrentSong();
                Songs.Clear();
                _currentSongIndex = 0;

                NameSong = "No songs found";
            }
            else
            {
                StopAndCleanCurrentSong();
                Songs.Clear();
                _currentSongIndex = 0;
                NameSong = "No songs found";
            }
        }
        [RelayCommand] async Task DeleteSong(SongVM songVM)
        {
            if (await _settingMediaPlayerService.DeleteSong(songVM.Id))
            {
                var songIndex = Songs.IndexOf(songVM);
                if (songIndex == -1) return;
                bool isCurrentSong = _currentSongIndex == songIndex;
                Songs.RemoveAt(songIndex);
                songVM.Data = null;

                if (_currentSongIndex > songIndex) _currentSongIndex--;
                else if (_currentSongIndex == songIndex)
                {
                    StopAndCleanCurrentSong();
                    if (Songs.Any())
                    {
                        _currentSongIndex = _currentSongIndex % Songs.Count;
                        await PlaySongOptimized(_currentSongIndex);
                    }
                    else
                    {
                        _currentSongIndex = 0;
                        NameSong = "No songs found";
                    }
                    return;
                }

                if (Songs.Any() && _currentSongIndex >= 0 && _currentSongIndex < Songs.Count) UpdateNameSong(_currentSongIndex);
                else NameSong = string.Empty;
            }
        }

        [RelayCommand] void SetPosition(double newPosition)
        {
            if (_audioReader != null && Duration > 0 && _isUserSeeking)
            {
                var timeSpan = TimeSpan.FromSeconds(newPosition);
                if (_audioReader.CanSeek)
                {
                    _audioReader.CurrentTime = timeSpan;
                    CurrentTime = FormatTime(newPosition);
                }
            }
        }
        [RelayCommand] void StartSeeking() => _isUserSeeking = true;
        [RelayCommand] void StopSeeking(object parameter)
        {
            _isUserSeeking = false;
            if (parameter is double finalPosition && _audioReader != null && Duration > 0)
            {
                var timeSpan = TimeSpan.FromSeconds(finalPosition);
                if (_audioReader.CanSeek)
                {
                    _audioReader.CurrentTime = timeSpan;
                    Position = finalPosition;
                    CurrentTime = FormatTime(finalPosition);
                }
            }
        }

        async Task PlaySong(SongVM songvm)
        {
            if (songvm?.Data == null) return;

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DisposeAudioResources();
                    _currentAudioStream = new MemoryStream(songvm.Data);
                    _currentAudioStream.Position = 0;

                    try { _audioReader = new Mp3FileReader(_currentAudioStream); }
                    catch
                    {
                        try
                        {
                            _currentAudioStream.Position = 0;
                            _audioReader = new WaveFileReader(_currentAudioStream);
                        }
                        catch
                        {
                            _currentAudioStream.Position = 0;
                            var rawSource = new RawSourceWaveStream(_currentAudioStream, new WaveFormat(44100, 16, 2));
                            _audioReader = rawSource;
                        }
                    }

                    _outputDevice.Init(_audioReader);

                    if (_audioReader.TotalTime != TimeSpan.Zero)
                    {
                        Duration = _audioReader.TotalTime.TotalSeconds;
                        TotalTime = FormatTime(Duration);
                    }
                    _outputDevice.Play();
                });

                _isPlaying = true;
                _positionTimer.Start();
            }
            catch (Exception ex) { _isPlaying = false; }
        }
        async Task PlaySong(int index)
        {
            if (index < 0 || index >= Songs.Count) return;

            try
            {
                var song = Songs[index];

                if (song.Data == null) song.Data = await SetMetaData(song.Id).ConfigureAwait(false);

                await PlaySong(song);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateNameSong(index);
                    Position = 0;
                    CurrentTime = "0:00";
                });
            }
            catch (Exception ex) { }
        }

        void InitializeAudioPlayer()
        {

            if (_outputDevice != null)
            {
                _outputDevice.PlaybackStopped -= OnPlaybackStopped;
                _outputDevice.Dispose();
            }

            _outputDevice = new WaveOutEvent();
            _outputDevice.Volume = (float)Volume;
            _outputDevice.PlaybackStopped += OnPlaybackStopped;
        }
        async void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                DisposeAudioResources();
                _isPlaying = false;
                return;
            }

            if (!_isPlaying || Songs.Count == 0) return;

            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                if (_isRepeat)
                {
                    StopAndCleanCurrentSong();
                    await PlaySong(_currentSongIndex);
                }
                else
                {
                    ClearCurrentSongData();
                    _currentSongIndex = (_currentSongIndex + 1) % Songs.Count;
                    await PlaySong(_currentSongIndex);
                }
            });
        }
        async Task<byte[]> SetMetaData(Guid id)
        {
            var arrayBytes = await _settingMediaPlayerService.GetArrayBytesById(id);
            return arrayBytes;
        }
        void UpdateNameSong(int index)
        {
            if (index >= 0 && index < Songs.Count) NameSong = Songs[index].Title ?? "Unknown Song";
        }
        partial void OnVolumeChanged(double value)
        {
            if (_outputDevice != null) _outputDevice.Volume = (float)value;
        }
        void UpdatePosition(object sender, EventArgs e)
        {
            if (!_isUserSeeking && _audioReader != null && _outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Position = _audioReader.CurrentTime.TotalSeconds;
                CurrentTime = FormatTime(Position);
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
        void StopCurrentSong()
        {
            try
            {
                _outputDevice?.Stop();
                _positionTimer?.Stop();
                _isPlaying = false;
            }
            catch (Exception ex) { }
        }
        void StopAndCleanCurrentSong()
        {
            try
            {
                _outputDevice?.Stop();
                _outputDevice?.Dispose();
                _outputDevice = null;

                _positionTimer?.Stop();
                _isPlaying = false;

                DisposeAudioResources();
                ClearCurrentSongData();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                InitializeAudioPlayer();
            }
            catch { }
        }
        void DisposeAudioResources()
        {
            try
            {
                _audioReader?.Dispose();
                _audioReader = null;

                _currentAudioStream?.Dispose();
                _currentAudioStream = null;
            }
            catch { }
        }
        void ClearCurrentSongData()
        {
            try
            {
                if (_currentSongIndex >= 0 && _currentSongIndex < Songs.Count)
                {
                    var currentSong = Songs[_currentSongIndex];
                    if (currentSong != null) currentSong.Data = null;
                }

                var nextIndex = (_currentSongIndex + 1) % Songs.Count;
                var itemsToRemove = _songCache.Keys.Where(k => k != nextIndex).ToList();

                foreach (var key in itemsToRemove) _songCache.Remove(key);
            }
            catch (Exception ex) { _songCache?.Clear(); }
        }

        Dictionary<int, byte[]> _songCache = new();
        async Task PlaySongOptimized(int index)
        {
            if (index < 0 || index >= Songs.Count) return;

            try
            {
                var song = Songs[index];
                byte[] songData;

                if (_songCache.ContainsKey(index))
                {
                    songData = _songCache[index];
                    _songCache.Remove(index);
                }
                else if (song.Data != null) songData = song.Data;
                else songData = await SetMetaData(song.Id);

                if (songData != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        DisposeAudioResources();
                        _currentAudioStream = new MemoryStream(songData);
                        _currentAudioStream.Position = 0;

                        try { _audioReader = new Mp3FileReader(_currentAudioStream); }
                        catch
                        {
                            try
                            {
                                _currentAudioStream.Position = 0;
                                _audioReader = new WaveFileReader(_currentAudioStream);
                            }
                            catch
                            {
                                _currentAudioStream.Position = 0;
                                var rawSource = new RawSourceWaveStream(_currentAudioStream, new WaveFormat(44100, 16, 2));
                                _audioReader = rawSource;
                            }
                        }
                        _outputDevice.Init(_audioReader);

                        if (_audioReader.TotalTime != TimeSpan.Zero)
                        {
                            Duration = _audioReader.TotalTime.TotalSeconds;
                            TotalTime = FormatTime(Duration);
                        }
                        _outputDevice.Play();
                        UpdateNameSong(index);
                        Position = 0;
                        CurrentTime = "0:00";
                    });

                    _isPlaying = true;
                    _positionTimer.Start();
                }
                _ = PreloadNextSong();
            }
            catch (Exception ex) { }
        }
        async Task PreloadNextSong()
        {
            try
            {
                var nextIndex = (_currentSongIndex + 1) % Songs.Count;
                if (nextIndex < Songs.Count && !_songCache.ContainsKey(nextIndex))
                {
                    var nextSong = Songs[nextIndex];
                    if (nextSong.Data == null)
                    {
                        var data = await SetMetaData(nextSong.Id);

                        if (_songCache.Count >= 2)
                        {
                            var oldestKey = _songCache.Keys.First();
                            _songCache.Remove(oldestKey);
                        }
                        _songCache[nextIndex] = data;
                    }
                    else
                    {
                        if (_songCache.Count >= 2)
                        {
                            var oldestKey = _songCache.Keys.First();
                            _songCache.Remove(oldestKey);
                        }
                        _songCache[nextIndex] = nextSong.Data;
                    }
                }
            }
            catch (Exception ex) { }
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
                    StopCurrentSong();

                    if (_outputDevice != null) _outputDevice.PlaybackStopped -= OnPlaybackStopped;

                    DisposeAudioResources();
                    _outputDevice?.Dispose();
                    _outputDevice = null;

                    if (_positionTimer != null)
                    {
                        _positionTimer.Stop();
                        _positionTimer.Tick -= UpdatePosition;
                        _positionTimer = null;
                    }

                    Songs?.Clear();
                    _songCache?.Clear();
                    ClearCurrentSongData();
                }
                _disposed = true;
            }
        }

        ~MediaPlayerService() => Dispose(false);
    }
}