using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public class CompositeViewModel(IViewService viewService, ITabService tabService, IMediaPlayerFactory mediaPlayerFactory, 
        IMessenger messenger, IHardNoteService hardNoteService, ICategoryNoteService categoryNoteService, ICommandService commandService)
    {
        public CompositeHeaderViewModel Header { get; set; } = new(mediaPlayerFactory, commandService);
        public CompositeMenuViewModel Menu { get; set; } = new(viewService, tabService, messenger, hardNoteService, categoryNoteService);
        public CompositeMainViewModel Main { get; set; } = new(tabService, commandService);
    }
}