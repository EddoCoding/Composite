using CommunityToolkit.Mvvm.Messaging;
using Composite.Services;

namespace Composite.ViewModels
{
    public class CompositeHeaderViewModel(IViewService viewService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
    {
        public string Title { get; set; } = "Composite";
        public HeaderMediaPlayerViewModel HeaderMediaPlayerViewModel { get; set; } = new(viewService, messenger, settingMediaPlayerService);
    }
}