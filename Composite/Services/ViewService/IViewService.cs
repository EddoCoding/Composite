using System.Windows.Controls;

namespace Composite.Services
{
    public interface IViewService
    {
        void Register<View, ViewModel>() where ViewModel : class;
        void ShowView<ViewModel>();
        UserControl GetUserControl<ViewModel>();
        void CloseView<ViewModel>();
    }
}