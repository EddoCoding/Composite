using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class AddCategoryNoteViewModel(IViewService viewService, IMessenger messenger, ICategoryNoteService categoryNoteService) : ObservableObject
    {
        public string Title { get; set; } = "Добавление категории";
        public CategoryNoteVM CategoryNoteVM { get; set; } = new();

        [RelayCommand] async void AddCategory()
        {
            if(await categoryNoteService.AddCategory(CategoryNoteVM))
            {
                messenger.Send(new CategoryNoteMessage(CategoryNoteVM));
                Close();
            }
        }
        [RelayCommand] void Close() => viewService.CloseView<AddCategoryNoteViewModel>();
    }
}