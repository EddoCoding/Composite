using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes.HardNote;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class ChangeHardNoteViewModel : ObservableObject
    {
        readonly Guid _id;
        readonly IViewService _viewService;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;

        [ObservableProperty] HardNoteVM hardNoteVM;

        public ChangeHardNoteViewModel(IViewService viewService, ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService)
        {
            _viewService = viewService;
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;

            messenger.Register<ChangeHardNoteMessage>(this, (r, m) =>
            {
                CopyHardNoteVM(m.HardNoteVM);
                messenger.Unregister<ChangeHardNoteMessage>(this);
            });
        }

        [RelayCommand] async Task UpdateHardNote()
        {
            if (await _hardNoteService.UpdateHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new ChangeHardNoteBackMessage(HardNoteVM));
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