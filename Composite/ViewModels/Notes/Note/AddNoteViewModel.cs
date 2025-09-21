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
    public partial class AddNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;
        [ObservableProperty] string _message;
        [ObservableProperty] CategoryNoteVM selectedCategory;

        public NoteVM NoteVM { get; set; } = new NoteVM();
        public List<string> Fonts { get; }
        public List<double> FontSizes { get; }
        public ObservableCollection<CategoryNoteVM> Categories { get; }

        public AddNoteViewModel(ITabService tabService, IMessenger messenger, INoteService noteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;

            Fonts = new(System.Windows.Media.Fonts.SystemFontFamilies.Select(x => x.Source).OrderBy(x => x));
            FontSizes = new() { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            Categories = new(categoryNoteService.GetCategories());
            SelectedCategory = Categories?.FirstOrDefault();

            messenger.Register<CheckNoteBackMessage>(this, (r, m) => 
            { 
                if (m.TitleNote && _id == m.Id)
                {
                    NoteVM.Category = SelectedCategory.NameCategory;
                    AddNote();
                }
                if (_id == m.Id) Message = m.ErrorMessage;
            });
            messenger.Register<CategoryNoteMessage>(this, (r, m) =>
            {
                Categories.Add(m.CategoryNote);
            });
            messenger.Register<DeleteCategoryNoteMessage>(this, (r, m) =>
            {
                if(SelectedCategory.NameCategory == m.Category.NameCategory) SelectedCategory = Categories?.FirstOrDefault();
                Categories.Remove(m.Category);
            });
        }

        [RelayCommand] void CheckNote() => _messenger.Send(new CheckNoteMessage(_id, NoteVM.Title));

        async void AddNote() 
        {
            if (await _noteService.AddNoteAsync(NoteVM))
            {
                _messenger.Send(new NoteMessage(NoteVM));
                _tabService.RemoveTab(this);
            }
        }

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