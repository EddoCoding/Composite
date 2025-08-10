using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;

namespace Composite.ViewModels
{
    public partial class SettingMediaPlayerViewModel : ObservableObject
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        readonly ISettingMediaPlayerService _settingMediaPlayerService;

        public string Title { get; set; } = "Настройка медиаплеера";

        [ObservableProperty] string pathFolder;

        public SettingMediaPlayerViewModel(IViewService viewService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
        {
            _viewService = viewService;
            _messenger = messenger;
            _settingMediaPlayerService = settingMediaPlayerService;

            PathFolder = settingMediaPlayerService.GetPath();
        }

        [RelayCommand] void SelectPathFolder() => PathFolder = _settingMediaPlayerService.SelectPathFolder();
        [RelayCommand] async void Save()
        {
            if (await _settingMediaPlayerService.InsertUpdatePath(PathFolder)) 
            {
                _messenger.Send(new PathFolderMessage(PathFolder));
                Close();
            }
        }
        [RelayCommand] void Close() => _viewService.CloseView<SettingMediaPlayerViewModel>();
    }
}