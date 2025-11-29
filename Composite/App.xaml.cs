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

                connection.Execute("Create Table If Not Exists HardNotes(Id Text Primary Key, Title Text Default '', DateCreate DateTime Not Null, Category Text, Password Text)");
                connection.Execute("Create Table If Not Exists CompositeBase(Id Text Primary Key, HardNoteId Text, ParentId Text, CompositeType Text, Tag Text, Comment Text, OrderIndex Integer, " +
                    "Foreign Key(HardNoteId) References HardNotes(Id) On Delete Cascade, " +
                    "Foreign Key(ParentId) References CompositeBase(Id) On Delete Cascade)");

                connection.Execute("Create Table If Not Exists TextComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists HeaderComposites(Id Text Primary Key, Text Text, FontWeight Text, FontSize Integer, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists QuoteComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists CodeComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists MarkerComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists NumericComposites(Id Text Primary Key, Number Integer, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists TaskComposites(Id Text Primary Key, Completed Integer, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists TasksComposites(Id Text Primary Key, Text Text, Completed Integer, Status Text, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists SubTaskComposites(Id Text Primary Key, Text Text, Completed Integer, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists LineComposites(Id Text Primary Key, LineSize Integer, LineColor Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists DocumentComposites(Id Text Primary Key, Text Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists ReferenceComposites(Id Text Primary Key, Text Text, ValueRef Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists ReferencesComposites(Id Text Primary Key, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists ImageComposites(Id Text Primary Key, HorizontalAlignment Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists FormattedTextComposites(Id Text Primary Key, BorderSize Integer, CornerRadius Integer, BorderColor Text, BackgroundColor Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
                connection.Execute("Create Table If Not Exists SongComposites(Id Text Primary Key, Title Text, Data Blob)");
                
                connection.Execute("Create Table If Not Exists DocumentsComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");              
                connection.Execute("Create Table If Not Exists DocumentMiniComposites(Id Text Primary Key, Text Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");

                connection.Execute("Create Table If Not Exists Songs(Id Text Primary Key, Title Text, Data Blob)");
                connection.Execute("Create Table if Not Exists Categories(NameCategory Text Primary Key)");
                connection.Execute("Insert Or Ignore Into Categories(NameCategory) Values(@NameCategory)", new { NameCategory = "Uncategorized" });
            }
        }
    }
}