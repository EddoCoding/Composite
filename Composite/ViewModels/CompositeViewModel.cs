using CommunityToolkit.Mvvm.Messaging;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public class CompositeViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, ISettingMediaPlayerService settingMediaPlayerService)
    {
        public CompositeHeaderViewModel Header { get; set; } = new(viewService, messenger, settingMediaPlayerService);
        public CompositeMenuViewModel Menu { get; set; } = new(viewService, tabService);
        public CompositeMainViewModel Main { get; set; } = new(tabService);
    }
}