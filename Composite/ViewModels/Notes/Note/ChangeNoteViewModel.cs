using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using System.Reflection;
using System.Windows.Media;

namespace Composite.ViewModels.Notes
{
    public partial class ChangeNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;

        [ObservableProperty] NoteVM noteVM;
        [ObservableProperty] string message;
        public List<string> Fonts { get; }
        public List<double> FontSizes { get; }
        public List<string> Categories { get; set; }
        public List<string> Colors { get; set; }
        [ObservableProperty] string selectedColor = "White";
        [ObservableProperty] string namePasswordButton = "Установить пароль";

        public ChangeNoteViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, INoteService noteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;

            Fonts = new(System.Windows.Media.Fonts.SystemFontFamilies.Select(x => x.Source).OrderBy(x => x));
            FontSizes = new() { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            Categories = new(categoryNoteService.GetCategoryNotes());
            Colors = new();
            Colors = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).Select(x => x.Name).ToList();

            messenger.Register<ChangeNoteMessage>(this, (r, m) => 
            {
                SelectedColor = m.NoteVM.Color;
                CopyNoteVM(m.NoteVM);
                if (!string.IsNullOrEmpty(m.NoteVM.Password)) NamePasswordButton = "Сбросить пароль";

                messenger.Unregister<ChangeNoteMessage>(this); //Чтобы данные открытой заметки для изменения не появлялись в предыдущих уже открытых заметках
            });
            messenger.Register<PasswordNoteBackMessage>(this, (r, m) =>
            {
                if (NoteVM.Id == m.Id)
                {
                    NoteVM.Password = m.Password;
                    NamePasswordButton = "Сбросить пароль";
                }
            });
            messenger.Register<CheckChangeNoteBackMessage>(this, (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote) { ChangeNote(); }
                    else Message = m.ErrorMessage;
                }
            });
        }

        [RelayCommand] async Task CheckNote() => _messenger.Send(new CheckChangeNoteMessage(_id, NoteVM.Id, NoteVM.Title));
        [RelayCommand] void CheckPassword()
        {
            if (string.IsNullOrEmpty(NoteVM.Password) && OpenViewSetPassword()) _messenger.Send(new PasswordNoteMessage(NoteVM.Id));
            else
            {
                NoteVM.Password = string.Empty;
                NamePasswordButton = "Установить пароль";
            }
        }

        async void ChangeNote()
        {
            NoteVM.Color = SelectedColor;
            if (await _noteService.UpdateNoteAsync(NoteVM))
            {
                _messenger.Send(new ChangeNoteBackMessage(NoteVM));
                _tabService.RemoveTab(this);
            }
        }
        bool OpenViewSetPassword() => _viewService.ShowView<SetPasswordViewModel>();
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
                    Categories.Clear();
                    Colors.Clear();
                    _messenger.UnregisterAll(this);
                }
                _disposed = true;
            }
        }
    }
}