using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Common.Message.Notes;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.HardNote;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes
{
    public partial class NotesViewModel : ObservableObject, IDisposable
    {
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;
        readonly ICategoryNoteService _categoryNoteService;
        ObservableCollection<NoteBaseVM> _allNotes = new();

        NoteButton _noteButton = new();
        public ObservableCollection<NoteBaseVM> Notes { get; set; } = new();
        public NotesManagementViewModel NotesManagementViewModel { get; set; }

        public NotesViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, INoteService noteService, 
            IHardNoteService hardNoteService, ICategoryNoteService categoryNoteService)
        {
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;
            _hardNoteService = hardNoteService;
            _categoryNoteService = categoryNoteService;

            NotesManagementViewModel = new(viewService, messenger, categoryNoteService);

            //Сообщения простых заметок
            messenger.Register<InputPasswordBackMessage>(this, (r, m) =>
            {
                NoteBaseVM? noteVM = Notes.FirstOrDefault(x => x.Id == m.Id);
                if (noteVM is NoteVM notevm) OpenNote(notevm);
            });
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
            });
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
            });
            messenger.Register<NoteMessage>(this, (r, m) => 
            {
                _allNotes.Add(m.NoteVM);
                Notes.Insert(Notes.Count - 1, m.NoteVM);
            });
            messenger.Register<ChangeNoteBackMessage>(this, (r, m) => 
            {
                NoteBaseVM? noteVM = Notes.FirstOrDefault(x => x.Id == m.NoteVM.Id);
                if (noteVM is NoteVM notevm)
                {
                    notevm.Title = m.NoteVM.Title;
                    notevm.Content = m.NoteVM.Content;
                    notevm.DateCreate = m.NoteVM.DateCreate;
                    notevm.Password = m.NoteVM.Password;
                    notevm.Preview = m.NoteVM.Preview;
                    notevm.FontFamily = m.NoteVM.FontFamily;
                    notevm.FontSize = m.NoteVM.FontSize;
                    notevm.Category = m.NoteVM.Category;
                    notevm.Color = m.NoteVM.Color;
                }
            });
            messenger.Register<InputPasswordDeleteBackMessage>(this, async (r, m) => 
            {
                NoteBaseVM? noteVM = Notes.FirstOrDefault(x => x.Id == m.Id);
                if (await _noteService.DeleteNoteAsync(noteVM.Id)) Notes.Remove(noteVM);
            });

            //Сообщения функциональных заметок
            messenger.Register<AddHardNoteMessage>(this, (r, m) =>
            {
                _allNotes.Add(m.HardNoteVM);
                Notes.Insert(Notes.Count - 1, m.HardNoteVM);
            });

            GetNotes();
            GetHardNotes();
            GetButtonAddNote();
        }

        [RelayCommand] void SelectTypeNote() => _viewService.ShowView<SelectTypeNoteViewModel>();
        [RelayCommand] async void DeleteNote(NoteBaseVM noteVM)
        {
            if (!string.IsNullOrEmpty(((NoteVM)noteVM).Password))
            {
                CheckPassword(noteVM);
                return;
            }
            if(await _noteService.DeleteNoteAsync(noteVM.Id))
            {
                _allNotes.Remove(noteVM as NoteVM);
                Notes.Remove(noteVM);
            }
        }
        [RelayCommand] async void DuplicateNote(NoteVM noteVM)
        {
            var noteVMDuplicate = await _noteService.DuplicateNoteVM(noteVM);
            if(noteVMDuplicate != null) Notes.Insert(Notes.Count - 1, noteVMDuplicate);
        }
        [RelayCommand] void CheckPasswordNote(NoteVM noteVM)
        {
            if (string.IsNullOrEmpty(noteVM.Password)) OpenNote(noteVM);
            else if(_viewService.ShowView<InputPasswordViewModel>()) _messenger.Send(new InputPasswordMessage(noteVM.Id, noteVM.Password));
        }
        [RelayCommand] async void DeleteCategory(string nameCategory)
        {
            if (await _categoryNoteService.DeleteCategory(nameCategory))
            {
                NotesManagementViewModel.DeleteCategory(nameCategory);
                var notesVM = Notes
                    .OfType<NoteVM>()
                    .Where(x => x.Category == nameCategory);

                foreach (var noteVM in notesVM) noteVM.Category = "Без категории";
            }
        }
        [RelayCommand] void SelectedCategory(string nameCategory)
        {
            Notes.Clear();

            if(nameCategory == "Все")
            {
                foreach (var noteVM in _allNotes) Notes.Add(noteVM);
                Notes.Add(_noteButton);
                return;
            }

            var notesVM = _allNotes.Where(x => x.Category == nameCategory);
            foreach (var noteVM in notesVM) Notes.Add(noteVM);
            Notes.Add(_noteButton);
        }
        [RelayCommand] void SearchNote()
        {
            Notes.Clear();
            var notesVM = _allNotes.Where(x => x.Title.Contains(NotesManagementViewModel.TextSearch));
            foreach (var noteVM in notesVM) Notes.Add(noteVM);
            Notes.Add(_noteButton);
        }

        void OpenNote(NoteVM noteVM)
        {
            if(_tabService.CreateTab<ChangeNoteViewModel>($"{noteVM.Title}")) _messenger.Send(new ChangeNoteMessage(noteVM));
        }
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
        void GetButtonAddNote() => Notes.Add(_noteButton);
        void CheckPassword(NoteBaseVM noteVM)
        {
            var notevm = noteVM as NoteVM;
            if(_viewService.ShowView<InputPasswordDeleteViewModel>()) _messenger.Send(new InputPasswordDeleteMessage(notevm.Id, notevm.Password));
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
                NotesManagementViewModel.Dispose();
                _messenger.UnregisterAll(this);
                _allNotes?.Clear();
                Notes?.Clear();
                _disposed = true;
            }
        }
    }
}