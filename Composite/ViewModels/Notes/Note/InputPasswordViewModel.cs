using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes.Note;
using Composite.Services;

namespace Composite.ViewModels.Notes
{
    public partial class InputPasswordViewModel : IDisposable
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        Guid _id;
        string _password;

        public string Title { get; set; } = "Ввод пароля";
        public string Password { get; set; } = string.Empty;

        public InputPasswordViewModel(IViewService viewService, IMessenger messenger)
        {
            _viewService = viewService;
            _messenger = messenger;

            messenger.Register<InputPasswordMessage>(this, (r, m) => { (_id, _password) = (m.Id, m.Password); });
        }

        [RelayCommand] void InputPassword()
        {
            if(_password == Password)
            {
                _messenger.Send(new InputPasswordBackMessage(_id));
                Close();
            }
        }
        [RelayCommand] void Close() => _viewService.CloseView<InputPasswordViewModel>();

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