using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Message.Notes;
using Composite.Common.Message.Notes.Note;
using Composite.Services;
using Composite.Services.TabService;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class ChangeHardNoteViewModel : ObservableObject, IDisposable
    {
        readonly Guid _id;
        readonly ITabService _tabService;
        readonly IMessenger _messenger;
        readonly IHardNoteService _hardNoteService;
        readonly ICommandService _commandService;
        [ObservableProperty] string _message;
        [ObservableProperty] HardNoteVM _hardNoteVM;
        [ObservableProperty] CategoryNoteVM selectedCategory;
        [ObservableProperty] string _passwordVisible = "Collapsed";
        CancellationTokenSource _messageCts;

        public ObservableCollection<CategoryNoteVM> Categories { get; }

        public ChangeHardNoteViewModel(ITabService tabService, IMessenger messenger, IHardNoteService hardNoteService, 
            ICategoryNoteService categoryNoteService, ICommandService commandService)
        {
            _id = Guid.NewGuid();
            _tabService = tabService;
            _messenger = messenger;
            _hardNoteService = hardNoteService;
            _commandService = commandService;

            HardNoteVM = new(tabService, hardNoteService, messenger);
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
            messenger.Register<CheckChangeNoteBackMessage>(this, async (r, m) =>
            {
                if (_id == m.Id)
                {
                    if (m.TitleNote) 
                    {
                        HardNoteVM.Category = SelectedCategory.NameCategory;
                        UpdateHardNote(); 
                    }
                    else
                    {
                        _messageCts?.Cancel();
                        _messageCts = new CancellationTokenSource();

                        Message = m.ErrorMessage;

                        try
                        {
                            await Task.Delay(3000, _messageCts.Token);
                            Message = null;
                        }
                        catch (TaskCanceledException) { }
                    }
                }
            });
            messenger.Register<CategoryNoteMessage>(this, (r, m) =>
            {
                Categories.Add(m.CategoryNote);
            });
            messenger.Register<DeleteCategoryNoteMessage>(this, (r, m) =>
            {
                if (SelectedCategory.NameCategory == m.Category.NameCategory) SelectedCategory = Categories?.FirstOrDefault();
                var category = Categories.FirstOrDefault(x => x.NameCategory == m.Category.NameCategory);
                if(category != null) Categories.Remove(category);
            });
        }

        [RelayCommand] void CheckNote() => _messenger.Send(new CheckChangeNoteMessage(_id, HardNoteVM.Id, HardNoteVM.Title));

        public void ExecuteCommand(string keyboardShortcut) => _commandService.ExecuteCommand(keyboardShortcut);
        async Task UpdateHardNote()
        {
            if (await _hardNoteService.UpdateHardNoteAsync(HardNoteVM))
            {
                _messenger.Send(new ChangeNoteBackMessage(HardNoteVM));
                _tabService.RemoveTab(this);
            }
        }
        [RelayCommand] void ShowPassword()
        {
            if (PasswordVisible == "Collapsed") PasswordVisible = "Visible";
            else PasswordVisible = "Collapsed";
        }
        [RelayCommand] void CloseTab() => _tabService.RemoveTab(this);

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
                if (disposing)
                {
                    _messageCts?.Cancel();
                    _messageCts?.Dispose();
                    _messenger.UnregisterAll(this);
                    SelectedCategory = null;
                    Categories.Clear();
                    HardNoteVM.StopAllSongs();
                }
                _disposed = true;
            }
        }
    }
}