using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using System.Collections.ObjectModel;

namespace Composite.ViewModels
{
    public partial class CompositeMenuViewModel : ObservableObject, IDisposable
    {
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;
        readonly ICategoryNoteService _categoryNoteService;

        ObservableCollection<NoteBaseVM> _allNotes = new();
        public ObservableCollection<NoteBaseVM> Notes { get; set; } = new();
        public ObservableCollection<CategoryNoteVM> Categories { get; set; }

        [ObservableProperty] bool _isPopupOpen;
        [ObservableProperty] bool _isPasswordPopupOpen;
        [ObservableProperty] string _password;
        [ObservableProperty] NoteBaseVM _note;
        [ObservableProperty] string _identifier;

        public CompositeMenuViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, 
            INoteService noteService, IHardNoteService hardNoteService, ICategoryNoteService categoryNoteService)
        {
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;
            _hardNoteService = hardNoteService;
            _categoryNoteService = categoryNoteService;

            Categories = new(categoryNoteService.GetCategories());

            messenger.Register<CheckNoteMessage>(this, (r, m) =>
            {
                if (string.IsNullOrEmpty(m.TitleNote))
                {
                    messenger.Send(new CheckNoteBackMessage(m.Id, false, "Пустой заголовок."));
                    return;
                }
            
                var checkTitleNote = Notes.FirstOrDefault(x => x.Title == m.TitleNote);
                if (checkTitleNote != null) messenger.Send(new CheckNoteBackMessage(m.Id, false, "Заметка с таким заголовком уже существует."));
                else messenger.Send(new CheckNoteBackMessage(m.Id, true));
            });         //Для валидации создания простой заметки
            messenger.Register<CheckChangeNoteMessage>(this, (r, m) =>
            {
                if (string.IsNullOrEmpty(m.TitleNote))
                {
                    messenger.Send(new CheckChangeNoteBackMessage(m.Id, false, "Пустой заголовок."));
                    return;
                }

                var note = Notes.FirstOrDefault(x => x.Id == m.IdNote && x.Title == m.TitleNote);
                if (note != null)
                {
                    messenger.Send(new CheckChangeNoteBackMessage(m.Id, true));
                    return;
                }

                var checkTitleNote = Notes.FirstOrDefault(x => x.Title == m.TitleNote);
                if (checkTitleNote != null) messenger.Send(new CheckChangeNoteBackMessage(m.Id, false, "Заметка с таким заголовком уже существует."));
                else messenger.Send(new CheckChangeNoteBackMessage(m.Id, true));
            });   //Для валидации изменения простой заметки
            messenger.Register<NoteMessage>(this, (r, m) =>
            {
                _allNotes.Add(m.Note);
                Notes.Insert(Notes.Count, m.Note);
            });              //Для добавления заметки в меню заметок
            messenger.Register<ChangeNoteBackMessage>(this, (r, m) =>
            {
                NoteBaseVM? noteVM = Notes.FirstOrDefault(x => x.Id == m.Note.Id);
                if (noteVM is NoteVM note)
                {
                    var noteMessage = (NoteVM)m.Note;

                    note.Title = noteMessage.Title;
                    note.Content = noteMessage.Content;
                    note.Category = noteMessage.Category;
                    note.DateCreate = noteMessage.DateCreate;
                    note.FontFamily = noteMessage.FontFamily;
                    note.FontSize = noteMessage.FontSize;
                    note.Password = noteMessage.Password;
                }
                else if (noteVM is HardNoteVM hardNote)
                {
                    var noteMessage = (HardNoteVM)m.Note;

                    hardNote.Title = noteMessage.Title;
                    hardNote.Category = noteMessage.Category;
                    hardNote.DateCreate = noteMessage.DateCreate;
                    hardNote.Password = noteMessage.Password;
                    hardNote.Composites = noteMessage.Composites;
                }
            });    //Для обновления данных уже загруженно заметки
            messenger.Register<CategoryNoteMessage>(this, (r, m) =>
            {
                Categories.Add(m.CategoryNote);
            });      //Для добавления заметки после ее добавления

            GetAddButtonNote();
            GetNotes();
            GetHardNotes();
        }

        [RelayCommand] void SelectTypeNote() => _viewService.ShowView<SelectTypeNoteViewModel>();
        void OpenNote(NoteBaseVM note)
        {
            if(note is NoteVM)
            {
                if (_tabService.CreateTab<ChangeNoteViewModel>($"{note.Title}")) _messenger.Send(new ChangeNoteMessage(note));
            }
            else if(note is HardNoteVM)
            {
                if (_tabService.CreateTab<ChangeHardNoteViewModel>($"{note.Title}")) _messenger.Send(new ChangeNoteMessage(note));
            }
        }
        [RelayCommand] void OpenPopupPassword((NoteBaseVM note, string identifier) parameters)
        {
            if (parameters.note.Password != string.Empty)
            {
                Note = parameters.note;
                Identifier = parameters.identifier;
                IsPasswordPopupOpen = true;
            }
            else if(parameters.identifier == "Open")
            {
                ClearPopupPassword();
                OpenNote(parameters.note);
                Note = null;
            }
            else if(parameters.identifier == "Delete")
            {
                ClearPopupPassword();
                DeleteNote(parameters.note);
                Note = null;
            }
        }
        [RelayCommand] void CheckPassword()
        {
            if(_note.Password != Password)
            {
                Password = string.Empty;
                return;
            }
            if (_note.Password == Password && Identifier == "Open")
            {
                ClearPopupPassword();
                OpenNote(_note);
                Note = null;
            }
            else if(_note.Password == Password && Identifier == "Delete")
            {
                ClearPopupPassword();
                DeleteNote(_note);
                Note = null;
            }
        }
        void ClearPopupPassword()
        {
            IsPasswordPopupOpen = false;
            Password = string.Empty;
            Identifier = string.Empty;
        }

        //Фичи заметок
        [RelayCommand] void OpenClosePopup()
        {
            if (IsPopupOpen == false) IsPopupOpen = true;
            else IsPopupOpen = false;
        }
        [RelayCommand] void ShowAllCategories()
        {
            Notes.Clear();
            GetAddButtonNote();
            foreach (var note in _allNotes) Notes.Add(note);

            OpenClosePopup();
        }
        [RelayCommand] void SortByTitle()
        {
            var notes = _allNotes.OrderBy(x => x.Title);
            Notes.Clear();
            GetAddButtonNote();
            foreach (var note in notes) Notes.Add(note);

            OpenClosePopup();
        }
        [RelayCommand] void SortByDateCreate()
        {
            var notes = _allNotes.OrderBy(x => x.DateCreate);
            Notes.Clear();
            GetAddButtonNote();
            foreach (var note in notes) Notes.Add(note);

            OpenClosePopup();
        }
        [RelayCommand] void SortByCategory(CategoryNoteVM categoryVM)
        {
            var notes = _allNotes.Where(x => x.Category == categoryVM.NameCategory);
            Notes.Clear();
            GetAddButtonNote();
            foreach (var note in notes) Notes.Add(note);

            OpenClosePopup();
        }
        [RelayCommand] void OpenAddCategoryNoteView() => _viewService.ShowView<AddCategoryNoteViewModel>();
        [RelayCommand] async Task DeleteCategory(CategoryNoteVM categoryVM)
        {
            if(await _categoryNoteService.DeleteCategory(categoryVM.NameCategory))
            {
                var category = Categories.FirstOrDefault(x => x.NameCategory == categoryVM.NameCategory);
                if(category != null)
                {
                    Categories.Remove(category);
                    _messenger.Send(new DeleteCategoryNoteMessage(categoryVM));

                    if (await _categoryNoteService.SetCategory(categoryVM.NameCategory))
                    {
                        _allNotes
                            .Where(x => x.Category == categoryVM.NameCategory)
                            .ToList()
                            .ForEach(note => note.Category = "Без категории");
                    }

                }
            }
        }

        //Команды меню заметок
        [RelayCommand] async Task DuplicateNote(NoteBaseVM noteVM)
        {
            if(noteVM is NoteVM note)
            {
                var noteDuplicate = await _noteService.DuplicateNoteVM(note);
                if (noteDuplicate != null)
                {
                    _allNotes.Add(noteDuplicate);
                    Notes.Insert(Notes.Count, noteDuplicate);
                }
            }
            if (noteVM is HardNoteVM hardNote)
            {
                var noteDuplicate = await _hardNoteService.DuplicateHardNoteVM(hardNote);
                if (noteDuplicate != null)
                {
                    _allNotes.Add(noteDuplicate);
                    Notes.Insert(Notes.Count, noteDuplicate);
                }
            }
        }
        [RelayCommand] async Task DeleteNote(NoteBaseVM noteVM)
        {
            if(noteVM is NoteVM)
            {
                if (await _noteService.DeleteNoteAsync(noteVM.Id))
                {
                    _allNotes.Remove(noteVM);
                    Notes.Remove(noteVM);

                    _messenger.Send(new RefMessage(noteVM.Id));
                }
            }
            if (noteVM is HardNoteVM note)
            {
                if (await _hardNoteService.DeleteHardNoteAsync(noteVM.Id))
                {
                    _allNotes.Remove(noteVM);
                    Notes.Remove(noteVM);

                    note.Dispose();

                    _messenger.Send(new RefMessage(noteVM.Id));
                }
            }

            _tabService.RemoveTab(noteVM.Title);
        }

        [RelayCommand] void Scale() => _viewService.ScaleView<CompositeViewModel>();
        [RelayCommand] void Collapse() => _viewService.CollapseView<CompositeViewModel>();
        [RelayCommand] void Close() => _viewService.CloseView<CompositeViewModel>();

        void GetNotes()
        {
            foreach (var noteVM in _noteService.GetNotes())
            {
                _allNotes.Add(noteVM);
                Notes.Add(noteVM);
            }
        }
        void GetHardNotes()
        {
            foreach (var hardNoteVM in _hardNoteService.GetNotes())
            {
                _allNotes.Add(hardNoteVM);
                Notes.Add(hardNoteVM);
            }
        }
        void GetAddButtonNote() => Notes.Add(new NoteButton());

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
                _messenger.UnregisterAll(this);
                _allNotes?.Clear();
                Notes?.Clear();
                _disposed = true;
            }
        }
    }
}