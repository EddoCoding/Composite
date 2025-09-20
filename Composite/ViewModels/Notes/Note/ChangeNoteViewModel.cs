using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels.Notes
{
    public partial class ChangeNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;
        [ObservableProperty] NoteVM noteVM;
        [ObservableProperty] string message;

        public List<string> Fonts { get; }
        public List<double> FontSizes { get; }
        public List<CategoryNoteVM> Categories { get; }
        public CategoryNoteVM SelectedCategory { get; set; } = new();

        public ChangeNoteViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, INoteService noteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;

            Fonts = new(System.Windows.Media.Fonts.SystemFontFamilies.Select(x => x.Source).OrderBy(x => x));
            FontSizes = new() { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            Categories = new(categoryNoteService.GetCategories());

            messenger.Register<ChangeNoteMessage>(this, (r, m) =>
            {
                if (m.Note is NoteVM noteVM)
                {
                    CopyNoteVM(noteVM);
                    messenger.Unregister<ChangeNoteMessage>(this);
                    SelectedCategory = Categories?.FirstOrDefault(x => x.NameCategory == NoteVM.Category);
                }
            });
            messenger.Register<CheckChangeNoteBackMessage>(this, (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote)
                    {
                        NoteVM.Category = SelectedCategory.NameCategory;
                        ChangeNote();
                    }
                    else Message = m.ErrorMessage;
                }
            });
        }

        [RelayCommand] async Task CheckNote() => _messenger.Send(new CheckChangeNoteMessage(_id, NoteVM.Id, NoteVM.Title));

        async void ChangeNote()
        {
            if (await _noteService.UpdateNoteAsync(NoteVM))
            {
                _messenger.Send(new ChangeNoteBackMessage(NoteVM));
                _tabService.RemoveTab(this);
            }
        }

        void CopyNoteVM(NoteVM originalNoteVM) => NoteVM = _noteService.CreateNoteVM(originalNoteVM);

        bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Fonts.Clear();
                    FontSizes.Clear();
                    _messenger.UnregisterAll(this);
                }
                _disposed = true;
            }
        }
    }
}