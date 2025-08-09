using System.Collections.ObjectModel;

namespace Composite.Services.TabService
{
    public interface ITabService
    {
        ObservableCollection<TabViewModel> Tabs { get; set; }
        TabViewModel SelectedTab { get; set; }

        void CreateTab<ViewModel>(string titleTab);
        void RemoveTab(TabViewModel tab);
        void RemoveTab(object viewModel);
    }
}