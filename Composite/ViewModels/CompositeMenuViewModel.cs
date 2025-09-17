using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.ViewModels
{
    public partial class CompositeMenuViewModel(IViewService viewService, ITabService tabService)
    {

        public ObservableCollection<NoteBaseVM> Collections { get; set; } = new()
        {
            new NoteVM() { Title = "Простая заметка 1" },
            new NoteVM() { Title = "Простая заметка 2" },
            new NoteVM() { Title = "Простая заметка 3" },
            new NoteVM() { Title = "Простая заметка 4" },
            new NoteVM() { Title = "Простая заметка 5" },

            new HardNoteVM() { Title = "Функциональная заметка 1"},
            new HardNoteVM() { Title = "Функциональная заметка 2"}
        };

        [RelayCommand] void OpenNotes() => tabService.CreateTab<NotesViewModel>("Заметки");

        [RelayCommand] void Collapse() => viewService.CollapseView<CompositeViewModel>();
        [RelayCommand] void Close() => viewService.CloseView<CompositeViewModel>();
    }
}