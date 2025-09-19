using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.Note;

namespace Composite.ViewModels.Notes
{
    public partial class SelectTypeNoteViewModel(IViewService viewService, ITabService tabService) : ObservableObject
    {
        public string Title { get; } = "Note type selection";

        public string DescriptionDefaultNote { get; } = "1. Only text, only hardcore. \n" +
                                                        "2. Font Configuration (Type and Size). \n";

        public string DescriptionHardNote { get; } = "1. Text, headers, quote, line. \n";

        [RelayCommand] void OpenNote()
        {
            tabService.CreateTab<AddNoteViewModel>("Simple note");
            Close();
        }
        [RelayCommand] void OpenHardNote()
        {
            tabService.CreateTab<AddHardNoteViewModel>("Hard note");
            Close();
        }
        [RelayCommand] void Close() => viewService.CloseView<SelectTypeNoteViewModel>();
    }
}