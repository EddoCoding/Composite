using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.ViewModels
{
    public partial class CompositeMenuViewModel : ObservableObject, IDisposable
    {
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;

        ObservableCollection<NoteBaseVM> _allNotes = new();
        public ObservableCollection<NoteBaseVM> Notes { get; set; } = new();

        //Для надстроек
        public string TextSearch { get; set; } = string.Empty;
        [ObservableProperty] bool _isPopupOpen;

        public CompositeMenuViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, 
            INoteService noteService, IHardNoteService hardNoteService)
        {
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;
            _hardNoteService = hardNoteService;

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
                if (note != null) messenger.Send(new CheckChangeNoteBackMessage(m.Id, true));

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
                    note.DateCreate = noteMessage.DateCreate;
                    note.FontFamily = noteMessage.FontFamily;
                    note.FontSize = noteMessage.FontSize;
                }
                else if (noteVM is HardNoteVM hardNote)
                {
                    var noteMessage = (HardNoteVM)m.Note;

                    hardNote.Title = noteMessage.Title;
                    hardNote.Composites = noteMessage.Composites;
                }
            });    //Для обновления данных уже загруженно заметки

            GetAddButtonNote();
            GetNotes();
            GetHardNotes();
        }

        [RelayCommand] void SelectTypeNote() => _viewService.ShowView<SelectTypeNoteViewModel>();
        [RelayCommand] void SearchNote()
        {
            Notes.Clear();
            var notesVM = _allNotes.Where(x => x.Title.Contains(TextSearch));
            foreach (var noteVM in notesVM) Notes.Add(noteVM);
        }
        [RelayCommand] void OpenNote(NoteBaseVM note)
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

        //Фичи менюшки
        [RelayCommand] void OpenClosePopup()
        {
            if (IsPopupOpen == false) IsPopupOpen = true;
            else IsPopupOpen = false;
        }
        [RelayCommand] void SortByTitle()
        {
            var notes = _allNotes.OrderBy(x => x.Title);
            Notes.Clear();
            GetAddButtonNote();
            foreach (var note in notes) Notes.Add(note);

            OpenClosePopup();
        }


        //Команды меню
        [RelayCommand] async Task DeleteNote(NoteBaseVM noteVM)
        {
            if(noteVM is NoteVM)
            {
                if (await _noteService.DeleteNoteAsync(noteVM.Id))
                {
                    _allNotes.Remove(noteVM);
                    Notes.Remove(noteVM);
                }
            }
            if (noteVM is HardNoteVM)
            {
                if (await _hardNoteService.DeleteHardNoteAsync(noteVM.Id))
                {
                    _allNotes.Remove(noteVM);
                    Notes.Remove(noteVM);
                }
            }

            _tabService.RemoveTab(noteVM.Title);
        }
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