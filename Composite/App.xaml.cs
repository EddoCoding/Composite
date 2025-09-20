using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Common.Mappers;
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
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<INoteRepository, NoteRepository>();
            services.AddTransient<INoteMap, NoteMap>();
            services.AddTransient<INoteFactory, NoteFactory>();

            services.AddTransient<IHardNoteService, HardNoteService>();
            services.AddTransient<IHardNoteRepository, HardNoteRepository>();
            services.AddTransient<IHardNoteMap, HardNoteMap>();
            services.AddTransient<IHardNoteFactory, HardNoteFactory>();
        }

        void RegisterView()
        {
            //Общий UI
            _serviceView.Register<CompositeHeaderView, CompositeHeaderViewModel>();
            _serviceView.Register<CompositeView, CompositeViewModel>();
            _serviceView.Register<CompositeMenuView, CompositeMenuViewModel>();
            _serviceView.Register<CompositeMainView, CompositeMainViewModel>();

            //Заметки
            _serviceView.Register<AddNoteView, AddNoteViewModel>();
            _serviceView.Register<ChangeNoteView, ChangeNoteViewModel>();
            _serviceView.Register<SelectTypeNoteView, SelectTypeNoteViewModel>();

            _serviceView.Register<AddHardNoteView, AddHardNoteViewModel>();
            _serviceView.Register<ChangeHardNoteView, ChangeHardNoteViewModel>();
        }

        void InitializeDataBase()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                
                //Простые заметки
                var queryCreateNotes = "Create Table If Not Exists Notes(Id Text Primary Key, Title Text Not Null, Content Text, DateCreate DateTime Not Null, " +
                                       "FontFamily Text, FontSize Real)";
                var queryCreateHardNotes = "Create Table If Not Exists HardNotes(Id Text Primary Key, Title Text Default '', DateCreate DateTime Not Null)";
                var queryCreateComposites = "Create Table If Not Exists Composites(Id Text Primary Key, Tag Text, Comment Text, Text Text, Header Text, FontWeightHeader Text, FontSizeHeader Integer, Quote Text, " +
                                            "HardNoteId TEXT NOT NULL, CompositeType TEXT NOT NULL, Foreign Key (HardNoteId) References HardNotes(Id) On Delete Cascade)";

                //Песни
                var queryCreateSongs = "Create Table If Not Exists Songs(Id Text Primary Key, Title Text, Data Blob)";
                
                connection.Execute(queryCreateNotes);
                connection.Execute(queryCreateHardNotes);
                connection.Execute(queryCreateComposites);
                connection.Execute(queryCreateSongs);
            }
        }
    }
}