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
                                                        "2. Font Configuration (Type and Size). \n" +
                                                        "3. Select category. \n" +
                                                        "4. Set password. \n";                  

        public string DescriptionHardNote { get; } = "1. Text, headers, quotes, lines, tasks, \n" +
                                                     "    images, refs, markers, numerics, \n" +
                                                     "    codeBlocks, documents, \n" +
                                                     "    formattedText \n" +
                                                     "2. Select category. \n" +
                                                     "3. Set password. \n";

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