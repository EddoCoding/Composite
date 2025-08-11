using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.Repositories;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Tasks;
using Composite.Views;
using Composite.Views.Notes;
using Composite.Views.Tasks;
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

            _serviceView.ShowView<CompositeViewModel>();

            InitializeDatabase();
        }

        void Configure(ServiceCollection services)
        {
            //Общие сервисы
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<ITabService, TabService>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>();
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<ISettingMediaPlayerService, SettingMediaPlayerService>();
            services.AddTransient<ISettingMediaPlayerRepository, SettingMediaPlayerRepository>();
            services.AddTransient<ISongMap, SongMap>();

            //Заметки
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<ICategoryNoteService, CategoryNoteService>();
            services.AddTransient<INoteRepository, NoteRepository>();
            services.AddTransient<ICategoryNoteRepository, CategoryNoteRepository>();
            services.AddTransient<INoteMap, NoteMap>();
            services.AddTransient<ICategoryNoteMap, CategoryNoteMap>();
            services.AddTransient<INoteFactory, NoteFactory>();
        }

        void RegisterView()
        {
            //Общий UI
            _serviceView.Register<CompositeHeaderView, CompositeHeaderViewModel>();
            _serviceView.Register<CompositeView, CompositeViewModel>();
            _serviceView.Register<CompositeMenuView, CompositeMenuViewModel>();
            _serviceView.Register<CompositeMainView, CompositeMainViewModel>();
            _serviceView.Register<SettingMediaPlayerView, SettingMediaPlayerViewModel>();

            //Заметки
            _serviceView.Register<NotesView, NotesViewModel>();
            _serviceView.Register<AddNoteView, AddNoteViewModel>();
            _serviceView.Register<ChangeNoteView, ChangeNoteViewModel>();
            _serviceView.Register<SetPasswordView, SetPasswordViewModel>();
            _serviceView.Register<InputPasswordView, InputPasswordViewModel>();
            _serviceView.Register<InputPasswordDeleteView, InputPasswordDeleteViewModel>();
            _serviceView.Register<AddCategoryView, AddCategoryViewModel>();

            //Задачи
            _serviceView.Register<TasksView, TasksViewModel>();
        }

        void InitializeDatabase()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                
                var queryCreateNotes = "Create Table If Not Exists Notes(Id Text Primary Key, Title Text Not Null, Content Text, DateCreate DateTime Not Null, " +
                                       "Password Varchar(24), Preview Integer Default 0, FontFamily Text, FontSize Real, Category Text, Color Text)";
                var queryCreateCategory = "Create Table If Not Exists Categories(NameCategory Text Primary Key Not Null)";
                var queryInsertCategory = "Insert Or Ignore Into Categories(NameCategory) VALUES(@NameCategory)";
                var queryCreateSongs = "Create Table If Not Exists Songs(Id Text Primary Key, Title Text, Data Blob)";
                
                connection.Execute(queryCreateNotes);
                connection.Execute(queryCreateCategory);
                connection.Execute(queryInsertCategory, new { NameCategory = "Без категории" });
                connection.Execute(queryCreateSongs);
            }
        }
    }
}