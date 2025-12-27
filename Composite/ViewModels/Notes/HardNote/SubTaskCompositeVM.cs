using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class SubTaskCompositeVM : CompositeBaseVM, IDisposable
    {
        public Action callBack;

        [ObservableProperty] string _text = "Подзадача";
        [ObservableProperty] bool _isCompleted;

        public SubTaskCompositeVM(Action callBack)
        {
            Id = Guid.NewGuid();

            this.callBack = callBack;
        }

        [RelayCommand] void TaskCompleted() => callBack?.Invoke();


        public override object Clone() => new SubTaskCompositeVM(callBack) { Id = Guid.NewGuid(), Text = Text, IsCompleted = IsCompleted };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Text = string.Empty;
                IsCompleted = false;
                callBack = null;
            }
            base.Dispose(disposing);
        }
    }
}