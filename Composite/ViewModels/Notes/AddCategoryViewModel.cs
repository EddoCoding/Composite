using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class AddCategoryViewModel(IViewService viewService, IMessenger messenger)
    {
        public string Title { get; set; } = "Добавление категории";
        public string NameCategory { get; set; } = string.Empty;

        [RelayCommand] void AddCategory()
        {
            messenger.Send(new AddCategoryMessage(NameCategory));
            Close();
        }
        [RelayCommand] void Close() => viewService.CloseView<AddCategoryViewModel>();
    }
}
