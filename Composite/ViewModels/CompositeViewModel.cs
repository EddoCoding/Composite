using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public class CompositeViewModel(IViewService viewService, ITabService tabService)
    {
        public CompositeHeaderViewModel Header { get; set; } = new();
        public CompositeMenuViewModel Menu { get; set; } = new(viewService, tabService);
        public CompositeMainViewModel Main { get; set; } = new(tabService);
    }
}