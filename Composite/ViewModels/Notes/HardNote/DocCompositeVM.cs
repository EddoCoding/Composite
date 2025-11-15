using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class DocCompositeVM : CompositeBaseVM, IDisposable
    {
        readonly IHardNoteService _hardNoteService;

        [ObservableProperty] string _text = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public DocCompositeVM(IHardNoteService hardNoteService)
        {
            _hardNoteService = hardNoteService;

            Id = Guid.NewGuid();
        }

        [RelayCommand] void SelectDocument()
        {
            var tuple = _hardNoteService.SelectDocument();

            if(tuple.Item1 != string.Empty)
            {
                Text = tuple.Item1;
                Data = tuple.Item2;
            }
        }
        [RelayCommand] async Task OpenDocument()
        {
            if (string.IsNullOrEmpty(Text)) return;

            IsLoading = true;

            var result = await _hardNoteService.OpenDocument(Text, Data);
            if (result != null) Data = result;

            IsLoading = false;
        }

        public override object Clone() => new DocCompositeVM(_hardNoteService) { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
                Data = Array.Empty<byte>();
            }
            base.Dispose(disposing);
        }

        [ObservableProperty] bool _isLoading;
    }
}