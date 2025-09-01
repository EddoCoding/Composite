using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.ViewModels.Notes.Note
{
    public partial class AddHardNoteViewModel : ObservableObject, IDisposable
    {
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;

        public HardNoteVM HardNoteVM { get; set; } = new();

        public AddHardNoteViewModel(ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService)
        {
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;
        }

        [RelayCommand] async Task AddHardNote()
        {
            if(await _hardNoteService.AddHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new AddHardNoteMessage(HardNoteVM));
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

                }
                _disposed = true;
            }
        }
    }
}