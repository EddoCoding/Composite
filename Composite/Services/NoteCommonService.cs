using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using System.Diagnostics;

namespace Composite.Services
{
    public class NoteCommonService : INoteCommonService
    {
        readonly ITabService _tabService;
        readonly INoteService _noteService;
        readonly IHardNoteService _hardNoteService;
        readonly IMessenger _messenger;
        public NoteCommonService(ITabService tabService, INoteService noteService, IHardNoteService hardNoteService, IMessenger messenger)
        {
            _tabService = tabService;
            _noteService = noteService;
            _hardNoteService = hardNoteService;
            _messenger = messenger;
        }

        public async Task CheckValuRef(RefCompositeVM refComposite)
        {
            if (Guid.TryParse(refComposite.ValueRef, out Guid result)) OpenNote(result);
            else OpenURL(refComposite.Text);
        }
        async Task OpenNote(Guid result)
        {
            var noteVM = await _noteService.GetNoteById(result);
            if (noteVM != null)
            {
                if (_tabService.CreateTab<ChangeNoteViewModel>($"{noteVM.Title}")) _messenger.Send(new ChangeNoteMessage(noteVM));
                return;
            }

            var hardNoteVM = await _hardNoteService.GetNoteById(result);
            if (hardNoteVM != null)
            {
                if (_tabService.CreateTab<ChangeHardNoteViewModel>($"{hardNoteVM.Title}")) _messenger.Send(new ChangeNoteMessage(hardNoteVM));
                return;
            }
        }
        async Task OpenURL(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uriResult.AbsoluteUri,
                    UseShellExecute = true
                });
            }
        }
    }
}