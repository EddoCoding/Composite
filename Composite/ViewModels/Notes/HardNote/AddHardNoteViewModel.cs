using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.HardNote;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.Note
{
    public partial class AddHardNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;
        [ObservableProperty] string _message;
        [ObservableProperty] CategoryNoteVM selectedCategory;
        [ObservableProperty] string _passwordVisible = "Collapsed";
        CancellationTokenSource _messageCts;

        public HardNoteVM HardNoteVM { get; set; }
        public ObservableCollection<CategoryNoteVM> Categories { get; }

        public AddHardNoteViewModel(ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService, ICategoryNoteService categoryNoteService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;

            HardNoteVM = new(tabService, hardNoteService, messenger);
            Categories = new(categoryNoteService.GetCategories());
            SelectedCategory = Categories.FirstOrDefault();

            messenger.Register<CheckNoteBackMessage>(this, async (r, m) =>
            {
                if (m.TitleNote && _id == m.Id)
                {
                    HardNoteVM.Category = SelectedCategory.NameCategory;
                    SaveHardNote();
                }
                if (_id == m.Id)
                {
                    try
                    {
                        _messageCts?.Cancel();
                        _messageCts = new CancellationTokenSource();
                    }
                    catch { }
                    

                    Message = m.ErrorMessage;

                    try
                    {
                        await Task.Delay(3000, _messageCts.Token);
                        Message = null;
                    }
                    catch { }
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

        [RelayCommand] void CheckNote() => _messenger.Send(new CheckNoteMessage(_id, HardNoteVM.Title));

        async Task SaveHardNote()
        {
            if(await _hardNoteService.AddHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new NoteMessage(HardNoteVM));
                _tabService.RemoveTab(this);
            }
        }
        [RelayCommand] void ShowPassword()
        {
            if (PasswordVisible == "Collapsed") PasswordVisible = "Visible";
            else PasswordVisible = "Collapsed";
        }
        [RelayCommand] void CloseTab() => _tabService.RemoveTab(this);

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
                if (disposing)
                {
                    _messageCts?.Cancel();
                    _messageCts?.Dispose();
                    _messenger.UnregisterAll(this);
                    SelectedCategory = null;
                    Categories.Clear();
                }
                _disposed = true;
            }
        }
    }
}