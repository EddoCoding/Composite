using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using System.Collections.ObjectModel;

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
        [ObservableProperty] CategoryNoteVM selectedCategory;
        [ObservableProperty] string _passwordVisible = "Collapsed";
        CancellationTokenSource _messageCts;

        public List<string> Fonts { get; }
        public List<double> FontSizes { get; }
        public ObservableCollection<CategoryNoteVM> Categories { get; }

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
            messenger.Register<CheckChangeNoteBackMessage>(this, async (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote)
                    {
                        NoteVM.Category = SelectedCategory.NameCategory;
                        ChangeNote();
                    }
                    else
                    {
                        _messageCts?.Cancel();
                        _messageCts = new CancellationTokenSource();

                        Message = m.ErrorMessage;

                        try
                        {
                            await Task.Delay(3000, _messageCts.Token);
                            Message = null;
                        }
                        catch (TaskCanceledException) { }
                    }
                }
            });
            messenger.Register<CategoryNoteMessage>(this, (r, m) =>
            {
                Categories.Add(m.CategoryNote);
            });
            messenger.Register<DeleteCategoryNoteMessage>(this, (r, m) =>
            {
                if (SelectedCategory.NameCategory == m.Category.NameCategory) SelectedCategory = Categories?.FirstOrDefault();
                Categories.Remove(m.Category);
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
        [RelayCommand] void ShowPassword()
        {
            if (PasswordVisible == "Collapsed") PasswordVisible = "Visible";
            else PasswordVisible = "Collapsed";
        }
        [RelayCommand] void CloseTab() => _tabService.RemoveTab(this);

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
                    _messageCts?.Cancel();
                    _messageCts?.Dispose();
                    _messenger.UnregisterAll(this);
                }
                _disposed = true;
            }
        }
    }
}