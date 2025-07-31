using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;
using Composite.Services.TabService;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes
{
    public partial class NotesViewModel : ObservableObject
    {
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly INoteService _noteService;

        public ObservableCollection<NoteBaseVM> Notes { get; set; }

        public NotesViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, INoteService noteService)
        {
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _noteService = noteService;

            messenger.Register<InputPasswordBackMessage>(this, (r, m) =>
            {
                NoteBaseVM? noteVM = Notes.FirstOrDefault(x => x.Id == m.Id);
                if (noteVM is NoteVM notevm) OpenNote(notevm);
            });
            messenger.Register<NoteMessage>(this, (r, m) => { Notes.Insert(Notes.Count - 1, m.NoteVM); });
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
                }
            });

            Notes = new() { new NoteButton() };

            GetNotes();
        }

        [RelayCommand] void AddNote() => _tabService.CreateTab<AddNoteViewModel>("Новая заметка");
        [RelayCommand] async void DeleteNote(NoteBaseVM note)
        {
            if (await _noteService.DeleteNoteAsync(note.Id)) Notes.Remove(note);
        }
        [RelayCommand] async void DuplicateNote(NoteVM noteVM)
        {
            var noteVMDuplicate = await _noteService.DuplicateNoteVM(noteVM);
            if(noteVMDuplicate != null) Notes.Insert(Notes.Count - 1, noteVMDuplicate);
        }
        [RelayCommand] void CheckPasswordNote(NoteVM noteVM)
        {
            if (string.IsNullOrEmpty(noteVM.Password)) OpenNote(noteVM);
            else
            {
                _viewService.ShowView<InputPasswordViewModel>();
                _messenger.Send(new InputPasswordMessage(noteVM.Id, noteVM.Password));
            }
        }

        void OpenNote(NoteVM noteVM)
        {
            _tabService.CreateTab<ChangeNoteViewModel>($"{noteVM.Title}");
            _messenger.Send(new ChangeNoteMessage(noteVM));
        }
        void GetNotes()
        {
            foreach (var noteVM in _noteService.GetNotes()) Notes.Insert(Notes.Count - 1, noteVM);
        }
    }
}