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

        public ObservableCollection<string> Categories { get; set; } = new();

        public NotesManagementViewModel(IViewService viewService, IMessenger messenger, ICategoryNoteService categoryNoteService)
        {
            _viewService = viewService;
            _messenger = messenger;
            _categoryNoteService = categoryNoteService;

            messenger.Register<AddCategoryMessage>(this, (r, m) => { AddCategory(m.CategoryNoteVM); });

            GetCategories();
        }

        [RelayCommand] void OpenViewAddCategory() => _viewService.ShowView<AddCategoryViewModel>();
        [RelayCommand] void SelectedCategory() { }

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

        public void Dispose()
        {
            _messenger.UnregisterAll(this);
            Categories.Clear();
        }
    }
}