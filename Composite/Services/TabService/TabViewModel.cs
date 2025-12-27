using CommunityToolkit.Mvvm.Input;

namespace Composite.Services.TabService
{
    public partial class TabViewModel : IDisposable
    {
        public Guid Id { get; set; }
        public string TitleTab { get; set; }
        public object ContentTab { get; set; }

        public TabViewModel(Guid id, string titleTab, object contentTab) => (Id, TitleTab, ContentTab) = (id, titleTab, contentTab);

        public event Action<TabViewModel> OnClose;
        [RelayCommand] void CloseTab() => OnClose?.Invoke(this);


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
                    TitleTab = string.Empty;
                    OnClose = null;
                    ContentTab = null;
                }
                _disposed = true;
            }
        }
    }
}