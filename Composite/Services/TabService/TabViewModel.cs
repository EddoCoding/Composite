using CommunityToolkit.Mvvm.Input;

namespace Composite.Services.TabService
{
    public partial class TabViewModel
    {
        public string TitleTab { get; set; }
        public object ContentTab { get; set; }

        public TabViewModel(string titleTab, object contentTab) => (TitleTab, ContentTab) = (titleTab, contentTab);


        public event Action<TabViewModel> OnClose;
        [RelayCommand] void CloseTab() => OnClose?.Invoke(this);
    }
}