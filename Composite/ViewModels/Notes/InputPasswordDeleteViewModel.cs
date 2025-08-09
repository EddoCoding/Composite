using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class InputPasswordDeleteViewModel : IDisposable
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        Guid _id;
        string _password;

        public string Title { get; set; } = "Ввод пароля";
        public string Password { get; set; } = string.Empty;

        public InputPasswordDeleteViewModel(IViewService viewService, IMessenger messenger)
        {
            _viewService = viewService;
            _messenger = messenger;

            messenger.Register<InputPasswordDeleteMessage>(this, (r, m) => { (_id, _password) = (m.Id, m.Password); });
        }

        [RelayCommand] void InputPassword()
        {
            if (_password == Password)
            {
                _messenger.Send(new InputPasswordDeleteBackMessage(_id));
                Close();
            }
        }
        [RelayCommand] void Close() => _viewService.CloseView<InputPasswordDeleteViewModel>();

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
                if (disposing) _messenger.UnregisterAll(this);
                _disposed = true;
            }
        }
    }
}