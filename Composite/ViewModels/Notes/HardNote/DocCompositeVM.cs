using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class DocCompositeVM : CompositeBaseVM
    {
        [ObservableProperty] string _text = string.Empty;
        public byte[] Data { get; set; } = Array.Empty<byte>();

        public DocCompositeVM() => Id = Guid.NewGuid();

        [RelayCommand] void SelectDocument()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Office Documents|*.docx;*.xlsx;*.pptx;*.doc;*.xls;*.ppt|" +
                         "Word Documents|*.docx;*.doc|" +
                         "Excel Documents|*.xlsx;*.xls|" +
                         "PowerPoint Documents|*.pptx;*.ppt|" +
                         "All files (*.*)|*.*",
                Title = "Select Office Document"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Text = Path.GetFileName(openFileDialog.FileName);
                Data = File.ReadAllBytes(openFileDialog.FileName);
            }
        }
        [RelayCommand] async Task OpenDocument()
        {
            if (Text == string.Empty || Text == null) return;

            try
            {
                string extension = Path.GetExtension(Text);
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}{extension}");

                await File.WriteAllBytesAsync(tempFilePath, Data);

                var processInfo = new ProcessStartInfo
                {
                    FileName = tempFilePath,
                    UseShellExecute = true
                };

                Process process = Process.Start(processInfo);

                if (process != null) await Task.Run(() => process.WaitForExit());

                Data = await File.ReadAllBytesAsync(tempFilePath);

                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public override object Clone() => new DocCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text };
    }
}