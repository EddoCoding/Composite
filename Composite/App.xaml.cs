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

            //Заметки
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<INoteRepository, NoteRepository>();
            services.AddTransient<INoteMap, NoteMap>();
        }

        void RegisterView()
        {
            //Общий UI
            _serviceView.Register<CompositeHeaderView, CompositeHeaderViewModel>();
            _serviceView.Register<CompositeView, CompositeViewModel>();
            _serviceView.Register<CompositeMenuView, CompositeMenuViewModel>();
            _serviceView.Register<CompositeMainView, CompositeMainViewModel>();

            //Заметки
            _serviceView.Register<NotesView, NotesViewModel>();
            _serviceView.Register<AddNoteView, AddNoteViewModel>();
            _serviceView.Register<SetPasswordView, SetPasswordViewModel>();

            //Задачи
            _serviceView.Register<TasksView, TasksViewModel>();
        }

        void InitializeDatabase()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                
                var queryCreateNotes = "Create table if not exists Notes(Id Text primary key, Title Text Not Null, Content Text, DateCreate DateTime not null, Password Varchar(24), Preview Integer default 0)";
                //var queryCreateContentNote = "Create table if not exists ContentBase(Id Text primary key, NoteId Text Not Null, Text Text Not Null," +
                //    "ContentType Varchar(50) Not Null Default 'Text', Foreign Key (NoteId) References Notes(Id) On Delete Cascade)";
        
                connection.Execute(queryCreateNotes);
                //connection.Execute(queryCreateContentNote);
            }
        }
    }
}