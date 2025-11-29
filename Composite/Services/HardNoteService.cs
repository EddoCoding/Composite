using CommunityToolkit.Mvvm.Messaging;
using Composite.Common.Factories;
using Composite.Common.Mappers;
using Composite.Common.Message.Notes;
using Composite.Repositories;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Composite.Services
{
    public class HardNoteService : IHardNoteService
    {
        readonly ITabService _tabService;
        readonly IHardNoteRepository _hardNoteRepository;
        readonly IHardNoteMap _hardNoteMap;
        readonly IHardNoteFactory _hardNoteFactory;
        readonly IMessenger _messenger;

        public HardNoteService(ITabService tabService, IHardNoteRepository hardNoteRepository, IMessenger messenger)
        {
            _tabService = tabService;
            _hardNoteRepository = hardNoteRepository;
            _hardNoteFactory = new HardNoteFactory(tabService, this, messenger);
            _hardNoteMap = new HardNoteMap(tabService, this, messenger);
            _messenger = messenger;
        }

        public async Task<bool> AddHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = _hardNoteMap.MapToModel(hardNoteVM);

            try
            {
                if (await _hardNoteRepository.Create(hardNote)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteHardNoteAsync(Guid id)
        {
            try
            {
                if (await _hardNoteRepository.Delete(id.ToString())) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateHardNoteAsync(HardNoteVM hardNoteVM)
        {
            var hardNote = _hardNoteMap.MapToModel(hardNoteVM);

            try
            {
                if (await _hardNoteRepository.Update(hardNote)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<HardNoteVM> GetNotes()
        {
            List<HardNoteVM> hardNotesVM = new();

            try
            {
                var hardNotes = _hardNoteRepository.Read();
                foreach (var hardNote in hardNotes)
                {
                    var hardNoteVM = _hardNoteMap.MapToViewModel(hardNote);
                    hardNotesVM.Add(hardNoteVM);
                }
                return hardNotesVM;
            }
            catch (Exception)
            {
                return hardNotesVM;
            }
        }
        public IEnumerable<NoteIdTitle> GetIdTitleNotes()
        {
            List<NoteIdTitle> hardNotesIdTitle = new();

            try
            {
                var hardNotes = _hardNoteRepository.GetIdTitleNotes();
                foreach (var hardNote in hardNotes)
                {
                    var hardNoteIdTitle = _hardNoteMap.MapToHardNoteIdTitle(hardNote);
                    hardNotesIdTitle.Add(hardNoteIdTitle);
                }
                return hardNotesIdTitle;
            }
            catch (Exception)
            {
                return hardNotesIdTitle;
            }
        }
        public async Task<HardNoteVM> GetNoteById(Guid id)
        {
            try
            {
                var hardNote = await _hardNoteRepository.GetNoteById(id.ToString());
                if (hardNote == null) return null;

                var hardNoteVM = _hardNoteMap.MapToViewModel(hardNote);
                return hardNoteVM;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Для ссылки
        public async Task CheckValuRef<T>(T refComposite)
        {
            var valueRefProp = typeof(T).GetProperty("ValueRef");
            var textProp = typeof(T).GetProperty("Text");

            var valueRef = valueRefProp?.GetValue(refComposite)?.ToString();
            var text = textProp?.GetValue(refComposite)?.ToString();

            if (Guid.TryParse(valueRef, out Guid result)) OpenNote(result);
            else OpenURL(text);
        }
        async Task OpenNote(Guid result)
        {
            var hardNoteVM = await GetNoteById(result);
            if (hardNoteVM != null)
            {
                if (_tabService.CreateTab<ChangeHardNoteViewModel>($"{hardNoteVM.Title}")) _messenger.Send(new ChangeNoteMessage(hardNoteVM));
                return;
            }
        }
        async Task OpenURL(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = uriResult.AbsoluteUri,
                        UseShellExecute = true
                    });
                }
                catch (Win32Exception)
                {
                    MessageBox.Show($"Не удалось открыть URL.");
                }
            }
        }

        //Для документа
        (string, byte[]) IHardNoteService.SelectDocument()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Office & PDF Documents|*.docx;*.xlsx;*.pptx;*.doc;*.xls;*.ppt;*.pdf",
                Title = "Select document",
                FilterIndex = 1,
                CheckFileExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string Text = Path.GetFileName(openFileDialog.FileName);
                    byte[] Data = File.ReadAllBytes(openFileDialog.FileName);

                    return (Text, Data);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("У Вас нет прав на чтение этого документа.");
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Ошибка доступа.");
                }
            }

            return (string.Empty, Array.Empty<byte>());
        }
        public async Task<byte[]?> OpenDocument(string text, byte[] data)
        {
            string extension = Path.GetExtension(text);
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}{extension}");
            try
            {
                await File.WriteAllBytesAsync(tempFilePath, data);

                var processInfo = new ProcessStartInfo
                {
                    FileName = tempFilePath,
                    UseShellExecute = true
                };

                Process.Start(processInfo);

                byte[]? result = await WaitForFileAccessAndRead(tempFilePath);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при работе с документом: {ex.Message}");
                return null;
            }
            finally
            {
                TryDeleteFileWithRetry(tempFilePath);
            }
        }
        async Task<byte[]?> WaitForFileAccessAndRead(string filePath)
        {
            await Task.Delay(2000);

            DateTime lastModified = File.GetLastWriteTime(filePath);
            bool fileWasModified = false;
            int attemptsWithoutLock = 0;

            while (true)
            {
                await Task.Delay(1000);

                try
                {
                    DateTime currentModified = File.GetLastWriteTime(filePath);

                    if (currentModified > lastModified)
                    {
                        fileWasModified = true;
                        lastModified = currentModified;
                        attemptsWithoutLock = 0;
                    }

                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        attemptsWithoutLock++;
                        if (attemptsWithoutLock >= 2)
                        {
                            byte[] buffer = new byte[stream.Length];
                            await stream.ReadAsync(buffer, 0, (int)stream.Length);
                            return buffer;
                        }
                    }
                }
                catch (IOException)
                {
                    attemptsWithoutLock = 0;
                    continue;
                }
            }
        }
        async void TryDeleteFileWithRetry(string filePath)
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i < 15; i++)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            break;
                        }
                    }
                    catch (IOException)
                    {
                        await Task.Delay(1000);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }

        public async Task<HardNoteVM> DuplicateHardNoteVM(NoteBaseVM hardNoteVM)
        {
            var currentId = hardNoteVM.Id;
            var id = Guid.NewGuid();
            hardNoteVM.Id = id;
            var note = _hardNoteMap.MapToModelWithNewIdComposite((HardNoteVM)hardNoteVM);
            note.Title = note.Title + " (duplicate)";

            try
            {
                if (await _hardNoteRepository.Create(note))
                {
                    hardNoteVM.Id = currentId;
                    var duplicateHardNoteVM = _hardNoteMap.MapToViewModel(note);

                    return duplicateHardNoteVM;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM) => _hardNoteFactory.CreateHardNoteVM(hardNoteVM);
    }
}