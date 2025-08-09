using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;
using Composite.Services.TabService;
using System.Reflection;
using System.Windows.Media;

namespace Composite.ViewModels.Notes
{
    public partial class AddNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;

        public NoteVM NoteVM { get; set; } = new NoteVM();
        public List<string> Fonts { get; }
        public List<double> FontSizes { get; }
        public List<string> Categories { get; set; }
        public List<string> Colors { get; set; }
        [ObservableProperty] string selectedColor = "White";
        [ObservableProperty] string namePasswordButton = "Установить пароль";

        public AddNoteViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, INoteService noteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;

            Fonts = new(System.Windows.Media.Fonts.SystemFontFamilies
                .Select(x => x.Source)
                .OrderBy(x => x));

            FontSizes = new() { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

            Categories = new(categoryNoteService.GetCategoryNotes());

            Colors = new();
            Colors = typeof(Colors)
            .GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Select(prop => prop.Name)
            .ToList();

            messenger.Register<PasswordNoteBackMessage>(this, (r, m) => 
            {
                if(_id == m.Id)
                {
                    NoteVM.Password = m.Password;
                    NamePasswordButton = "Сбросить пароль";
                }
            });
        }

        [RelayCommand] async void AddNote()
        {
            NoteVM.Color = SelectedColor;
            if(await _noteService.AddNoteAsync(NoteVM))
            {
                _messenger.Send(new NoteMessage(NoteVM));
                _tabService.RemoveTab(this);
            }
        }
        [RelayCommand] void CheckPassword() 
        {
            if (string.IsNullOrEmpty(NoteVM.Password))
            {
                OpenViewSetPassword();
                _messenger.Send(new PasswordNoteMessage(_id));
            }
            else
            {
                NoteVM.Password = string.Empty;
                NamePasswordButton = "Установить пароль";
            }
        }
        [RelayCommand] void SetPreview()
        {
            if (NoteVM.Preview == true) NoteVM.Preview = false;
            else NoteVM.Preview = true;
        }

        void OpenViewSetPassword() => _viewService.ShowView<SetPasswordViewModel>();

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