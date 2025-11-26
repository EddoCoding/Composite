using CommunityToolkit.Mvvm.ComponentModel;
using Composite.Common.Helpers;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels
{
    public partial class CompositeMainViewModel : ObservableObject
    {
        //Для DragDrop в корзину
        [ObservableProperty] bool isDragging;
        public MyDragHandler DragHandler { get; }

        [ObservableProperty] ITabService _tabService;

        public CompositeMainViewModel(ITabService tabService, ICommandService commandService)
        {
            _tabService = tabService;

            DragHandler = new MyDragHandler(isDragging => IsDragging = isDragging);
        }
    }
}