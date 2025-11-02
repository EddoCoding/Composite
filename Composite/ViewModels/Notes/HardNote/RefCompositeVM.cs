using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Services;
using Composite.Services.TabService;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class RefCompositeVM : CompositeBaseVM, IDisposable
    {
        readonly ITabService _tabService;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;
        readonly IMessenger _messenger;

        public string ValueRef { get; set; } = string.Empty;
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] bool _isRefPopup;
        [ObservableProperty] bool _isNotesPopup;

        public ObservableCollection<NoteIdTitle> Notes { get; set; } = new();

        public RefCompositeVM(ITabService tabService, INoteService noteService, IHardNoteService hardNoteService, IMessenger messenger)
        {
            _tabService = tabService;
            _noteService = noteService;
            _hardNoteService = hardNoteService;
            _messenger = messenger;

            Id = Guid.NewGuid();

            foreach (var noteIdTitle in noteService.GetIdTitleNotes()) Notes.Add(noteIdTitle);
            foreach (var hardNoteIdTitle in hardNoteService.GetIdTitleNotes()) Notes.Add(hardNoteIdTitle);
        }

        [RelayCommand] void OpenRefPopup()
        {
            if (IsRefPopup == false) IsRefPopup = true;
            else IsRefPopup = false;
        }
        [RelayCommand] void OpenNotesPopup()
        {
            if (IsNotesPopup == false) IsNotesPopup = true;
            else IsNotesPopup = false;
        }
        [RelayCommand] async Task CheckValuRef(RefCompositeVM refComposite) => _hardNoteService.CheckValuRef(refComposite);
        [RelayCommand] void SelectNote(NoteIdTitle noteIdTitle)
        {
            ValueRef = noteIdTitle.Id.ToString();
            Text = noteIdTitle.Title;
            IsNotesPopup = false;
            IsRefPopup = false;
        }
        [RelayCommand] void ClearSelectedNote()
        {
            ValueRef = null;
            Text = null;
            IsNotesPopup = false;
            IsRefPopup = false;
        }

        public override object Clone() => new RefCompositeVM(_tabService, _noteService, _hardNoteService, _messenger) 
        { 
            Id = Guid.NewGuid(), 
            Tag = Tag, 
            Comment = Comment, 
            ValueRef = ValueRef, 
            Text = Text 
        };

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                ValueRef = string.Empty;
                Text = string.Empty;
                IsRefPopup = false;
                IsNotesPopup = false;
                Notes.Clear();
                Notes = null;
            }
            base.Dispose(disposing);
        }
    }
}
