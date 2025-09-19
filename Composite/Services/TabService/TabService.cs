using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Composite.Services.TabService
{
    public partial class TabService : ObservableObject, ITabService
    {
        public ObservableCollection<TabViewModel> Tabs { get; set; } = new();
        [ObservableProperty] TabViewModel selectedTab;

        IViewService _serviceView;
        public TabService(IViewService serviceView)
        {
            _serviceView = serviceView;
            Tabs.CollectionChanged += OnTabsChanged;
        }

        public bool CreateTab<ViewModel>(string titleTab)
        {
            var doubleTab = Tabs.FirstOrDefault(x => x.TitleTab == titleTab);
            if (doubleTab != null)
            {
                SelectedTab = doubleTab;
                return false;
            }

            var userControl = _serviceView.GetUserControl<ViewModel>();
            var tab = new TabViewModel(titleTab, userControl);
            SelectedTab = tab;
            Tabs.Add(tab);
            return true;
        }
        public void RemoveTab(TabViewModel tab)
        {
            var uc = (UserControl)tab.ContentTab;
            if(uc.DataContext is IDisposable vm) vm.Dispose();
            Tabs.Remove(tab);
        }
        public void RemoveTab(object viewModel)
        {
            foreach (var tab in Tabs)
            {
                var uc = (UserControl)tab.ContentTab;
                if (uc.DataContext == viewModel)
                {
                    RemoveTab(tab);
                    return;
                }
            }
        }
        public void RemoveTab(string tabTitle)
        {
            var tab = Tabs.FirstOrDefault(x => x.TitleTab == tabTitle);
            RemoveTab(tab);
        }

        void OnTabsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TabViewModel item in e.OldItems) item.OnClose -= RemoveTab;
            }

            if (e.NewItems != null)
            {
                foreach (TabViewModel item in e.NewItems) item.OnClose += RemoveTab;
            }

        }
    }
}