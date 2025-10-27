using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Services;
using Composite.Services.TabService;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class RefCompositeVM : CompositeBaseVM
    {
        readonly ITabService _tabService;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;
        readonly INoteCommonService _noteCommonService;
        readonly IMessenger _messenger;

        public string ValueRef { get; set; } = string.Empty;
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] bool _isRefPopup;
        [ObservableProperty] bool _isNotesPopup;

        public ObservableCollection<NoteIdTitle> Notes { get; } = new();

        public RefCompositeVM(ITabService tabService, INoteService noteService, IHardNoteService hardNoteService, IMessenger messenger)
        {
            _tabService = tabService;
            _noteService = noteService;
            _hardNoteService = hardNoteService;
            _noteCommonService = new NoteCommonService(tabService, noteService, hardNoteService, messenger);
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
        [RelayCommand] async Task CheckValuRef(RefCompositeVM refComposite) => _noteCommonService.CheckValuRef(refComposite);
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
    }
}
