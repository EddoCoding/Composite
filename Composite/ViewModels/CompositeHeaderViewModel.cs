using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Common.Factories;
using Composite.Services;

namespace Composite.ViewModels
{
    public partial class CompositeHeaderViewModel : ObservableObject
    {
        readonly IMediaPlayerFactory _mediaPlayerFactory;

        public string Title { get; set; } = "Composite";

        [ObservableProperty] string nameButtonStartStopMediaPlayer = "Turn on player";
        [ObservableProperty] IMediaPlayerService mediaPlayerService;
        [ObservableProperty] Visibility visibility = Visibility.Collapsed;

        public CompositeHeaderViewModel(IMediaPlayerFactory mediaPlayerFactory, ICommandService commandService)
        {
            _mediaPlayerFactory = mediaPlayerFactory;

            commandService.RegsiterCommand("CTRLSPACE", StarStopMediaPlayer);
        }

        [RelayCommand] void StarStopMediaPlayer()
        {
            if (MediaPlayerService == null)
            {
                MediaPlayerService = _mediaPlayerFactory.Create();
                Visibility = Visibility.Visible;
                NameButtonStartStopMediaPlayer = "Turn off player";
            }
            else
            {
                MediaPlayerService.Dispose();
                MediaPlayerService = null;
                Visibility = Visibility.Collapsed;
                NameButtonStartStopMediaPlayer = "Turn on player";
            }
        }
    }
}