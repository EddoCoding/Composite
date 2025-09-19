using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class ChangeHardNoteViewModel : ObservableObject
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;
        [ObservableProperty] string _message;
        [ObservableProperty] HardNoteVM _hardNoteVM;

        public ChangeHardNoteViewModel(ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;

            messenger.Register<ChangeNoteMessage>(this, (r, m) =>
            {
                if (m.Note is HardNoteVM hardNoteVM)
                {
                    CopyHardNoteVM(hardNoteVM);
                    messenger.Unregister<ChangeNoteMessage>(this);
                }
            });
            messenger.Register<CheckChangeNoteBackMessage>(this, (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote) { UpdateHardNote(); }
                    else Message = m.ErrorMessage;
                }
            });
        }

        [RelayCommand] void CheckNote() => _messenger.Send(new CheckChangeNoteMessage(_id, HardNoteVM.Id, HardNoteVM.Title));

        async Task UpdateHardNote()
        {
            if (await _hardNoteService.UpdateHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new ChangeNoteBackMessage(HardNoteVM));
                _tabService.RemoveTab(this);
            }
        }
        void CopyHardNoteVM(HardNoteVM originalHardNoteVM) => HardNoteVM = _hardNoteService.CreateHardNoteVM(originalHardNoteVM);

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
            }
        }
    }
}