using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class SetPasswordViewModel(IViewService viewService, IMessenger messenger)
    {
        public string Title { get; set; } = "Установка пароля";
        public string Password { get; set; } = string.Empty;

        [RelayCommand] void SetPassword()
        {
            messenger.Send(new PasswordNoteMessage(Password));
            Close();
        }
        [RelayCommand] void Close() => viewService.CloseView<SetPasswordViewModel>();
    }
}