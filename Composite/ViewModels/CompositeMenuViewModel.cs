using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Tasks;

namespace Composite.ViewModels
{
    public partial class CompositeMenuViewModel(IViewService viewService, ITabService tabService)
    {

        [RelayCommand] void OpenNotes() => tabService.CreateTab<NotesViewModel>("Заметки");
        [RelayCommand] void OpenTasks() => tabService.CreateTab<TasksViewModel>("Задачи");

        [RelayCommand] void Collapse() => viewService.CollapseView<CompositeViewModel>();
        [RelayCommand] void Close() => viewService.CloseView<CompositeViewModel>();
    }
}