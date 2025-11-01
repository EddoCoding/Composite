using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Composite.Services;
using System.Windows.Forms;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class DocCompositeVM : CompositeBaseVM
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

            Text = tuple.Item1;
            Data = tuple.Item2;
        }
        [RelayCommand] async Task OpenDocument()
        {
            if (string.IsNullOrEmpty(Text)) return;

            try
            {
                var result = await _hardNoteService.OpenDocument(Text, Data);
                if (result != null) Data = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public override object Clone() => new DocCompositeVM(_hardNoteService) { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
    }
}