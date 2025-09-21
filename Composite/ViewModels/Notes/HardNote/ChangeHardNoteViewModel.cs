using System.Collections.ObjectModel;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class ChangeHardNoteViewModel : ObservableObject
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;
        [ObservableProperty] string _message;
        [ObservableProperty] HardNoteVM _hardNoteVM;
        [ObservableProperty] CategoryNoteVM selectedCategory;

        public ObservableCollection<CategoryNoteVM> Categories { get; }

        public ChangeHardNoteViewModel(ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;

            Categories = new(categoryNoteService.GetCategories());

            messenger.Register<ChangeNoteMessage>(this, (r, m) =>
            {
                if (m.Note is HardNoteVM hardNoteVM)
                {
                    CopyHardNoteVM(hardNoteVM);
                    messenger.Unregister<ChangeNoteMessage>(this);
                    SelectedCategory = Categories?.FirstOrDefault(x => x.NameCategory == HardNoteVM.Category);
                }
            });
            messenger.Register<CheckChangeNoteBackMessage>(this, (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote) 
                    {
                        HardNoteVM.Category = SelectedCategory.NameCategory;
                        UpdateHardNote(); 
                    }
                    else Message = m.ErrorMessage;
                }
            });
            messenger.Register<CategoryNoteMessage>(this, (r, m) =>
            {
                Categories.Add(m.CategoryNote);
            });
            messenger.Register<DeleteCategoryNoteMessage>(this, (r, m) =>
            {
                if (SelectedCategory.NameCategory == m.Category.NameCategory) SelectedCategory = Categories?.FirstOrDefault();
                Categories.Remove(m.Category);
            });
        }

        [RelayCommand] void CheckNote() => _messenger.Send(new CheckChangeNoteMessage(_id, HardNoteVM.Id, HardNoteVM.Title));

        async Task UpdateHardNote()
        {
            if (await _hardNoteService.UpdateHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new ChangeNoteBackMessage(HardNoteVM));
                _tabService.RemoveTab(this);
            }
        }
        void CopyHardNoteVM(HardNoteVM originalHardNoteVM) => HardNoteVM = _hardNoteService.CreateHardNoteVM(originalHardNoteVM);

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
            }
        }
    }
}