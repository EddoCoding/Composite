using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels.Notes
{
    public partial class SelectTypeNoteViewModel(IViewService viewService, ITabService tabService) : ObservableObject
    {
        public string Title { get; } = "Выбор типа заметки";

        public string DescriptionDefaultNote { get; } = "1. Только текст. \n" +
                                                        "2. Настройка шрифта (тип, размер). \n" +
                                                        "3. Категоризация. \n" +
                                                        "4. Выбор цвета отображения заметки. \n" +
                                                        "5. Запароливание.\n" +
                                                        "6. Предварительный просмотр. \n";

        //public string DescriptionHardNote { get; } = "1. Только текст. \n" +
        //                                                "2. Настройка шрифта (тип, размер). \n" +
        //                                                "3. Категоризация. \n" +
        //                                                "4. Выбор цвета отображения заметки. \n" +
        //                                                "5. Запароливание.\n" +
        //                                                "6. Предварительный просмотр. \n" +
        //                                                "6. Предварительный просмотр. \n" +
        //                                                "6. Предварительный просмотр. \n" +
        //                                                "6. Предварительный просмотр. \n" +
        //                                                "6. Предварительный просмотр. \n" +
        //                                                "6. Предварительный просмотр. \n";

        [RelayCommand] void OpenDefaultNote()
        {
            tabService.CreateTab<AddNoteViewModel>("Простая заметка");
            Close();
        }
        [RelayCommand] void OpenHardNote()
        {
            tabService.CreateTab<AddHardNoteViewModel>("Функциональная заметка");
            Close();
        }

        [RelayCommand] void Close() => viewService.CloseView<SelectTypeNoteViewModel>();
    }
}