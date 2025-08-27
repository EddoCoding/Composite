using System.Windows.Controls;
using System.Windows;
using System.Reflection;

namespace Composite.Services
{
    public class ViewService(IServiceProvider serviceProvider) : IViewService
    {
        Dictionary<Type, Type> container = new Dictionary<Type, Type>();
        Dictionary<Window, object> openViews = new Dictionary<Window, object>();

        public void Register<View, ViewModel>() where ViewModel : class => container.Add(typeof(View), typeof(ViewModel));
        ViewModel CreateViewModel<ViewModel>()
        {
            var ctorViewModel = typeof(ViewModel).GetConstructors().FirstOrDefault();
            var ctorParameters = ctorViewModel?.GetParameters() ?? Array.Empty<ParameterInfo>();

            if (ctorParameters.Length == 0) return (ViewModel)Activator.CreateInstance(typeof(ViewModel));

            var dependencies = new List<object>();
            foreach (var parameter in ctorParameters)
            {
                var dependency = serviceProvider.GetService(parameter.ParameterType);
                dependencies.Add(dependency);
            }

            return (ViewModel)Activator.CreateInstance(typeof(ViewModel), dependencies.ToArray());
        }
        public bool ShowView<ViewModel>()
        {
            foreach (var item in container)
            {
                if (openViews.Any(kv => kv.Value.GetType() == typeof(ViewModel))) return false;
                if (item.Value == typeof(ViewModel))
                {
                    var view = (Window)Activator.CreateInstance(item.Key);
                    var viewModel = CreateViewModel<ViewModel>();

                    view.DataContext = viewModel;
                    view.Show();

                    openViews.Add(view, viewModel);
                    view.Closed += (sender, e) => OnViewClosed((Window)sender);

                    return true;
                }
            }
            return false;
        }
        void OnViewClosed(Window view)
        {
            if (openViews.TryGetValue(view, out var viewModel))
            {
                if (viewModel is IDisposable disposableViewModel) disposableViewModel.Dispose();
                openViews.Remove(view);
            }
        }
        public void CloseView<ViewModel>()
        {
            var viewToClose = openViews.FirstOrDefault(x => x.Value.GetType() == typeof(ViewModel));
            viewToClose.Key.Close();
        }
        public void CollapseView<ViewModel>()
        {
            var viewToCollapse = openViews.FirstOrDefault(kv => kv.Value.GetType() == typeof(ViewModel));
            viewToCollapse.Key.WindowState = WindowState.Minimized;
        }
        public UserControl GetUserControl<ViewModel>()
        {
            UserControl view = null;

            foreach (var item in container)
            {
                if (item.Value == typeof(ViewModel))
                {
                    view = (UserControl)Activator.CreateInstance(item.Key);
                    var ctorViewModel = typeof(ViewModel).GetConstructors().FirstOrDefault();
                    var ctorParameters = ctorViewModel.GetParameters();

                    List<object> dependencies = new List<object>();

                    foreach (var parameter in ctorParameters)
                    {
                        Type parameterType = parameter.ParameterType;
                        object dependency = serviceProvider.GetService(parameterType);
                        dependencies.Add(dependency);
                    }

                    var viewModel = Activator.CreateInstance(typeof(ViewModel), dependencies.ToArray());

                    view.DataContext = viewModel;

                    break;
                }
            }

            return view;
        }
    }
}