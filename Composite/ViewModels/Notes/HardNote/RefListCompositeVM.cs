using CommunityToolkit.Mvvm.Messaging;
using Composite.Services.TabService;
using Composite.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class RefListCompositeVM : CompositeBaseVM, IDisposable
    {
        readonly ITabService _tabService;
        readonly IHardNoteService _hardNoteService;
        readonly IMessenger _messenger;

        public ObservableCollection<ReferenceCompositeVM> References { get; set; } = new();

        public RefListCompositeVM(ITabService tabService, IHardNoteService hardNoteService, IMessenger messenger)
        {
            _tabService = tabService;
            _hardNoteService = hardNoteService;
            _messenger = messenger;

            Id = Guid.NewGuid();
        }

        [RelayCommand] void AddRefComposite() => References.Add(new ReferenceCompositeVM(_tabService, _hardNoteService, _messenger));
        [RelayCommand] void DeleteRefComposite(ReferenceCompositeVM referenceVM)
        {
            referenceVM.Dispose();
            References.Remove(referenceVM);
        }

        public override object Clone()
        {
            var refList = new RefListCompositeVM(_tabService, _hardNoteService, _messenger);

            foreach (var reference in References) refList.References.Add((ReferenceCompositeVM)reference.Clone());

            return refList;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var referenceVM in References) referenceVM.Dispose();
                References.Clear();
                References = null;
            }
            base.Dispose(disposing);
        }
    }
}