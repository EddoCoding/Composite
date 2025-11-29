using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using Composite.Services.Common;
using Microsoft.Win32;
using NAudio.Wave;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class SongCompositeVM : CompositeBaseVM, IDragSlider, IDisposable
    {
        [ObservableProperty] string _title = string.Empty;
        [ObservableProperty] double _position = 0;
        [ObservableProperty] double _duration = 0;
        [ObservableProperty] bool _isPlaying = false;
        [ObservableProperty] string _currentTime = "0:00";
        [ObservableProperty] string _totalTime = "0:00";

        public byte[] Data { get; set; }

        WaveOutEvent _waveOut;
        Mp3FileReader _mp3Reader;
        MemoryStream _memoryStream;
        DispatcherTimer _timer;
        bool _isDragging = false;

        public SongCompositeVM()
        {
            Id = Guid.NewGuid();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _timer.Tick += Timer_Tick;
        }

        [RelayCommand] async Task SelectSong()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "MP3 files|*.mp3",
                Title = "Select song"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                StopAndCleanup();

                var fileName = openFileDialog.FileName;
                Title = Path.GetFileNameWithoutExtension(fileName);
                Data = await File.ReadAllBytesAsync(fileName);

                InitializeAudio();
                if (_mp3Reader != null)
                {
                    Duration = _mp3Reader.TotalTime.TotalSeconds;
                    TotalTime = FormatTime(_mp3Reader.TotalTime);
                    CleanupAudio();
                }

                Position = 0;
                CurrentTime = "0:00";
            }
        }
        [RelayCommand] void PlayStopSong()
        {
            if (Data == null || Data.Length == 0) return;

            if (IsPlaying)
            {
                _waveOut?.Pause();
                _timer.Stop();
                IsPlaying = false;
            }
            else
            {
                if (_waveOut == null) InitializeAudio();

                _waveOut?.Play();
                _timer.Start();
                IsPlaying = true;
            }
        }

        partial void OnPositionChanged(double value)
        {
            if (_mp3Reader != null && _isDragging)
            {
                _mp3Reader.CurrentTime = TimeSpan.FromSeconds(value);
                CurrentTime = FormatTime(_mp3Reader.CurrentTime);
            }
        }
        public void StartDragging() => _isDragging = true;
        public void StopDragging() => _isDragging = false;
        void InitializeAudio()
        {
            try
            {
                CleanupAudio();

                _memoryStream = new MemoryStream(Data);
                _mp3Reader = new Mp3FileReader(_memoryStream);
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_mp3Reader);

                _waveOut.PlaybackStopped += OnPlaybackStopped;

                Duration = _mp3Reader.TotalTime.TotalSeconds;
                TotalTime = FormatTime(_mp3Reader.TotalTime);
            }
            catch (Exception) { }
        }
        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_mp3Reader != null && _mp3Reader.CurrentTime >= _mp3Reader.TotalTime)
            {
                _timer.Stop();
                IsPlaying = false;
                Position = 0;
                CurrentTime = "0:00";

                if (_mp3Reader != null)
                {
                    try
                    {
                        _mp3Reader.CurrentTime = TimeSpan.Zero;
                    }
                    catch (ObjectDisposedException) { }
                }
            }
        }
        void Timer_Tick(object sender, EventArgs e)
        {
            if (_mp3Reader != null && !_isDragging)
            {
                try
                {
                    Position = _mp3Reader.CurrentTime.TotalSeconds;
                    CurrentTime = FormatTime(_mp3Reader.CurrentTime);
                }
                catch (ObjectDisposedException) { _timer.Stop(); }
            }
        }
        string FormatTime(TimeSpan time) => $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
        void CleanupAudio()
        {
            if (_waveOut != null)
            {
                _waveOut.PlaybackStopped -= OnPlaybackStopped;
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
            }

            if (_mp3Reader != null)
            {
                _mp3Reader.Dispose();
                _mp3Reader = null;
            }

            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }
        }
        void StopAndCleanup()
        {
            _timer.Stop();
            IsPlaying = false;
            CleanupAudio();
            Position = 0;
            CurrentTime = "0:00";
            Duration = 0;
            TotalTime = "0:00";
        }
        public void Stop()
        {
            if (IsPlaying)
            {
                _waveOut?.Stop();
                _timer?.Stop();
                IsPlaying = false;
            }
        }


        public override object Clone() => new SongCompositeVM() { Id = Guid.NewGuid(), Title = Title, Position = Position, Duration = Duration,
            IsPlaying = IsPlaying, CurrentTime = CurrentTime, TotalTime = TotalTime, Data = Data };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAndCleanup();

                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= Timer_Tick;
                    _timer = null;
                }
                if (Data != null && Data.Length > 0)
                {
                    Array.Clear(Data, 0, Data.Length);
                    Data = null;
                }

                Title = string.Empty;
                Position = 0;
                Duration = 0;
                IsPlaying = false;
                CurrentTime = "0:00";
                TotalTime = "0:00";
            }
            base.Dispose(disposing);
        }
    }
}