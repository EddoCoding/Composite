using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public class CompositeViewModel(IViewService viewService, ITabService tabService, IMediaPlayerFactory mediaPlayerFactory, 
        IMessenger messenger, INoteService noteService, IHardNoteService hardNoteService)
    {
        public CompositeHeaderViewModel Header { get; set; } = new(mediaPlayerFactory);
        public CompositeMenuViewModel Menu { get; set; } = new(viewService, tabService, messenger, noteService, hardNoteService);
        public CompositeMainViewModel Main { get; set; } = new(tabService);
    }
}