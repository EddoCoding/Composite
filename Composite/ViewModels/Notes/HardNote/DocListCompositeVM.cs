using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using System.Collections.ObjectModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class DocListCompositeVM : CompositeBaseVM, IDisposable
    {
        readonly IHardNoteService _hardNoteService;

        [ObservableProperty] string _text = string.Empty;

        public ObservableCollection<DocumentCompositeVM> Documents { get; set; } = new();

        public DocListCompositeVM(IHardNoteService hardNoteService)
        {
            _hardNoteService = hardNoteService;

            Id = Guid.NewGuid();
        }

        [RelayCommand] void AddDocumentComposite() => Documents.Add(new DocumentCompositeVM(_hardNoteService));
        [RelayCommand] void DeleteDocumentComposite(DocumentCompositeVM documentVM)
        {
            documentVM.Dispose();
            Documents.Remove(documentVM);
        }

        public override object Clone()
        {
            var docList = new DocListCompositeVM(_hardNoteService)
            {
                Text = Text
            };

            foreach (var document in Documents) docList.Documents.Add((DocumentCompositeVM)document.Clone());

            return docList;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Text = string.Empty;
                foreach (var documentVM in Documents) documentVM.Dispose();
                Documents.Clear();
                Documents = null;
            }
            base.Dispose(disposing);
        }
    }
}