using CommunityToolkit.Mvvm.ComponentModel;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public partial class CompositeMainViewModel(ITabService tabService, ICommandService commandService) : ObservableObject
    {
        [ObservableProperty] ITabService _tabService = tabService;
    }
}