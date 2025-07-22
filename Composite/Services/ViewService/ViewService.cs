using System.Windows.Controls;
using System.Windows;

namespace Composite.Services
{
    public class ViewService(IServiceProvider serviceProvider) : IViewService
    {
        Dictionary<Type, Type> container = new Dictionary<Type, Type>();
        Dictionary<Window, Type> openViews = new Dictionary<Window, Type>();

        public void Register<View, ViewModel>() where ViewModel : class 
            => container.Add(typeof(View), typeof(ViewModel));

        public void ShowView<ViewModel>()
        {
            foreach (var item in container)
            {
                if (item.Value == typeof(ViewModel))
                {
                    var view = (Window)Activator.CreateInstance(item.Key);
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
                    view.Show();

                    openViews.Add(view, typeof(ViewModel));

                    break;
                }
            }
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

        public void CloseView<ViewModel>()
        {
            foreach (var view in openViews)
                if (view.Value == typeof(ViewModel))
                {
                    view.Key.Close();
                    openViews.Remove(view.Key);

                    break;
                }
        }
    }
}