using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message;
using Composite.Services;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes
{
    public partial class NotesManagementViewModel : IDisposable
    {
        readonly IViewService _viewService;
        readonly IMessenger _messenger;
        readonly ICategoryNoteService _categoryNoteService;

        public ObservableCollection<string> Categories { get; set; } 
        public string TextSearch { get; set; }

        public NotesManagementViewModel(IViewService viewService, IMessenger messenger, ICategoryNoteService categoryNoteService)
        {
            _viewService = viewService;
            _messenger = messenger;
            _categoryNoteService = categoryNoteService;

            messenger.Register<AddCategoryMessage>(this, (r, m) => { AddCategory(m.CategoryNoteVM); });

            Categories = new() { "Все" };
            GetCategories();
        }

        [RelayCommand] void OpenViewAddCategory() => _viewService.ShowView<AddCategoryViewModel>();
       
        async void AddCategory(CategoryNoteVM categoryNoteVM)
        {
            if(await _categoryNoteService.AddCategory(categoryNoteVM)) Categories.Add(categoryNoteVM.NameCategory);
        }
        void GetCategories()
        {
            foreach (var categoryVM in _categoryNoteService.GetCategoryNotes()) Categories.Add(categoryVM);
        }
        public void DeleteCategory(string nameCategory)
        {
            var category = Categories.FirstOrDefault(x => x == nameCategory);
            Categories.Remove(category);
        }

        bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _messenger.UnregisterAll(this);
                Categories.Clear();
                _disposed = true;
            }
        }
    }
}