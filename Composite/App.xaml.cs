using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.DataBase;
using Composite.Repositories;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using Composite.ViewModels.Notes.Note;
using Composite.Views;
using Composite.Views.Notes;
using Composite.Views.Notes.Notes;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Composite
{
    public partial class App : Application
    {
        readonly ServiceProvider _serviceProvider;
        readonly IViewService _serviceView;
        readonly IDbConnectionFactory _dbConnectionFactory;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            Configure(serviceCollection);
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _serviceView = _serviceProvider.GetService<IViewService>();
            _dbConnectionFactory = _serviceProvider.GetService<IDbConnectionFactory>();

            RegisterView();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeDataBase();

            _serviceView.ShowView<CompositeViewModel>();
        }

        void Configure(ServiceCollection services)
        {
            //Общие сервисы
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<ITabService, TabService>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            services.AddTransient<IMediaPlayerService, MediaPlayerService>();
            services.AddTransient<IMediaPlayerFactory, MediaPlayerFactory>();

            services.AddTransient<ISettingMediaPlayerService, SettingMediaPlayerService>();
            services.AddTransient<ISettingMediaPlayerRepository, SettingMediaPlayerRepository>();
            services.AddTransient<ISongMap, SongMap>();

            //Заметки
            services.AddTransient<IHardNoteService, HardNoteService>();
            services.AddTransient<IHardNoteRepository, HardNoteRepository>();
            services.AddTransient<IHardNoteMap, HardNoteMap>();
            services.AddTransient<IHardNoteFactory, HardNoteFactory>();

            services.AddTransient<ICategoryNoteService, CategoryNoteService>();
            services.AddTransient<ICategoryNoteRepository, CategoryNoteRepository>();
            services.AddTransient<ICategoryNoteMap, CategoryNoteMap>();

            services.AddSingleton<ICommandService, CommandService>();
        }

        void RegisterView()
        {
            //Общий UI
            _serviceView.Register<CompositeHeaderView, CompositeHeaderViewModel>();
            _serviceView.Register<CompositeView, CompositeViewModel>();
            _serviceView.Register<CompositeMenuView, CompositeMenuViewModel>();
            _serviceView.Register<CompositeMainView, CompositeMainViewModel>();

            //Заметки

            _serviceView.Register<AddHardNoteView, AddHardNoteViewModel>();
            _serviceView.Register<ChangeHardNoteView, ChangeHardNoteViewModel>();
            _serviceView.Register<AddCategoryNoteView, AddCategoryNoteViewModel>();
        }

        void InitializeDataBase()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                DataBaseInitializer.Initialize(connection);
            }
        }
    }
}