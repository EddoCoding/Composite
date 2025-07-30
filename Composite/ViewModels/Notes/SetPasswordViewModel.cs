using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class SetPasswordViewModel
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        Guid _id;

        public string Title { get; set; } = "Установка пароля";
        public string Password { get; set; } = string.Empty;

        public SetPasswordViewModel(IViewService viewService, IMessenger messenger)
        {
            _viewService = viewService;
            _messenger = messenger;

            messenger.Register<PasswordNoteMessage>(this, (r, m) => { _id = m.Id; });
        }

        [RelayCommand] void SetPassword()
        {
            _messenger.Send(new PasswordNoteBackMessage(_id, Password));
            Close();
        }
        [RelayCommand] void Close() => _viewService.CloseView<SetPasswordViewModel>();
    }
}