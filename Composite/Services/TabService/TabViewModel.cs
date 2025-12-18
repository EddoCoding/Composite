using CommunityToolkit.Mvvm.Input;

namespace Composite.Services.TabService
{
    public partial class TabViewModel
    {
        public Guid Id { get; set; }
        public string TitleTab { get; set; }
        public object ContentTab { get; set; }

        public TabViewModel(Guid id, string titleTab, object contentTab) => (Id, TitleTab, ContentTab) = (id, titleTab, contentTab);


        public event Action<TabViewModel> OnClose;
        [RelayCommand] void CloseTab() => OnClose?.Invoke(this);
    }
}