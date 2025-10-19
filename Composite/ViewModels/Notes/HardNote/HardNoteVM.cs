using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HardNoteVM : NoteBaseVM, IDisposable
    {
        public override string ItemType => "HardNote";
        public ObservableCollection<CompositeBaseVM> Composites { get; set; }

        public HardNoteVM()
        {
            Id = Guid.NewGuid();
            Composites = new();
            Composites.Add(new TextCompositeVM());
            DateCreate = DateTime.Now;
        }

        public void AddTextCompositeVM() => Composites.Add(new TextCompositeVM());
        public CompositeBaseVM AddComposite(CompositeBaseVM current, int caretIndex)
        {
            if (current is TextCompositeVM textComposite)
            {
                var newTextComposite = new TextCompositeVM { Text = string.Empty };
                int index = Composites.IndexOf(textComposite);
                //Если каретка в начале
                if (caretIndex == 0)
                {
                    if (string.IsNullOrEmpty(textComposite.Text))
                    {
                        Composites.Insert(index + 1, newTextComposite);
                        return newTextComposite;
                    }
                    else
                    {
                        Composites.Insert(index, newTextComposite);
                        return newTextComposite;
                    }
                }
                //Если каретка в конце
                else if (caretIndex == textComposite.Text.Length)
                {
                    Composites.Insert(index + 1, newTextComposite);
                    return newTextComposite;
                }
                //Если каретка между началом и концом
                else
                {
                    if (caretIndex >= 0 && caretIndex < textComposite.Text.Length)
                    {
                        var textAfter = textComposite.Text.Substring(caretIndex);
                        newTextComposite.Text = textAfter;
                        textComposite.Text = textComposite.Text.Substring(0, caretIndex);
                        Composites.Insert(index + 1, newTextComposite);
                        return newTextComposite;
                    }
                    else
                    {
                        Composites.Insert(index + 1, newTextComposite);
                        return newTextComposite;
                    }
                }
            }
            if (current is HeaderCompositeVM headerComposite)
            {
                var newTextComposite = new TextCompositeVM { Text = string.Empty };
                int index = Composites.IndexOf(headerComposite);
                Composites.Insert(index + 1, newTextComposite);
                return newTextComposite;
            }
            if (current is QuoteCompositeVM quoteComposite)
            {
                var newTextComposite = new TextCompositeVM { Text = string.Empty };
                int index = Composites.IndexOf(quoteComposite);
                Composites.Insert(index + 1, newTextComposite);
                return newTextComposite;
            }
            if (current is TaskCompositeVM taskComposite)
            {
                if(taskComposite.Text != string.Empty)
                {
                    var taskCompositeVM = new TaskCompositeVM();
                    int index = Composites.IndexOf(taskComposite);
                    Composites.Insert(index + 1, taskCompositeVM);
                    return taskCompositeVM;
                }
                else
                {
                    var textCompositeVM = new TextCompositeVM();
                    int index = Composites.IndexOf(taskComposite);
                    DeleteComposite(current);
                    Composites.Insert(index, textCompositeVM);
                    return textCompositeVM;
                }
            }

            return null;
        }
        public CompositeBaseVM CreateComposite(string value, CompositeBaseVM compositeBaseVM, int currentIndex)
        {
            string Value = value.Trim().ToLower();
            switch (Value)
            {
                case "/header":
                    int index = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var headerComposite = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 24
                    };
                    Composites.Insert(index, headerComposite);
                    return headerComposite;
                case "/header1":
                    int index1 = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var headerComposite1 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 24
                    };
                    Composites.Insert(index1, headerComposite1);
                    return headerComposite1;
                case "/header2":
                    int index2 = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var headerComposite2 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 22
                    };
                    Composites.Insert(index2, headerComposite2);
                    return headerComposite2;
                case "/header3":
                    int index3 = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var headerComposite3 = new HeaderCompositeVM
                    {
                        FontWeight = "Bold",
                        FontSize = 20
                    };
                    Composites.Insert(index3, headerComposite3);
                    return headerComposite3;
                case "/quote":
                    int indexQuote = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var quoteComposite = new QuoteCompositeVM();
                    Composites.Insert(indexQuote, quoteComposite);
                    return quoteComposite;
                case "/line":
                    int indexLine = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var lineComposite = new LineCompositeVM();
                    Composites.Insert(indexLine, lineComposite);
                    var textComposite = new TextCompositeVM();
                    Composites.Insert(indexLine + 1, textComposite);
                    return textComposite;
                case "/task":
                {
                    int indexTask = Composites.IndexOf(compositeBaseVM);
                    DeleteComposite(compositeBaseVM);
                    var taskComposite = new TaskCompositeVM();
                    Composites.Insert(indexTask, taskComposite);
                    return taskComposite;
                }
                case "/image":
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog()
                        {
                            Filter = "Image Files (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp|All files (*.*)|*.*",
                            Title = "Select Image"
                        };

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var bitmap = LoadBitmapImage(openFileDialog.FileName);

                            int indexImage = Composites.IndexOf(compositeBaseVM);
                            DeleteComposite(compositeBaseVM);
                            var imageComposite = new ImageCompositeVM() { ImageSource = bitmap };
                            Composites.Insert(indexImage, imageComposite);
                            var textComposite1 = new TextCompositeVM();
                            Composites.Insert(indexImage + 1, textComposite1);
                            return textComposite1;
                        }

                        return null;
                    }

                default: return null;
            }
        }
        [RelayCommand] public void DeleteComposite(CompositeBaseVM composite)
        {
            Composites.Remove(composite);
            IsOpenPopup = false;
        }
        void DuplicateComposite<T>(T composite) where T : CompositeBaseVM
        {
            var index = Composites.IndexOf(composite);
            var duplicateComposite = (T)composite.Clone();
            Composites.Insert(index + 1, duplicateComposite);
            CloseContextMenus();
        }
        void ChangeTypeComposite(CompositeBaseVM composite, string selectType)
        {
            var index = Composites.IndexOf(composite);

            string? text = null;
            if (composite is TextCompositeVM textComposite) text = textComposite.Text;
            else if (composite is HeaderCompositeVM headerComposite) text = headerComposite.Text;
            else if (composite is QuoteCompositeVM quoteComposite) text = quoteComposite.Text;
            else if (composite is TaskCompositeVM taskComposite) text = taskComposite.Text;

            CompositeBaseVM? newComposite = null;

            switch (selectType)
            {
                case "Text":
                    newComposite = new TextCompositeVM() { Text = text };
                    break;
                case "Header1":
                    newComposite = new HeaderCompositeVM() { Text = text, FontWeight = "Bold", FontSize = 24 };
                    break;
                case "Header2":
                    newComposite = new HeaderCompositeVM() { Text = text, FontWeight = "Bold", FontSize = 22 };
                    break;
                case "Header3":
                    newComposite = new HeaderCompositeVM() { Text = text, FontWeight = "Bold", FontSize = 20 };
                    break;
                case "Quote":
                    newComposite = new QuoteCompositeVM() { Text = text };
                    break;
                case "Task":
                    newComposite = new TaskCompositeVM() { Text = text };
                    break;
            }

            if (newComposite != null)
            {
                Composites.Remove(composite);
                Composites.Insert(index, newComposite);
                CloseContextMenus();
            }
        }

        public void InsertComposite(int index, CompositeBaseVM composite) => Composites.Insert(index, composite);
        public int GetIndexComposite(CompositeBaseVM composite) => Composites.IndexOf(composite);

        BitmapImage LoadBitmapImage(string filePath)
        {
            var bitmap = new BitmapImage();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.EndInit();
            }
            bitmap.Freeze();

            if (bitmap.DpiX != 96 || bitmap.DpiY != 96)
            {
                var dpiFixedBitmap = new BitmapImage();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    dpiFixedBitmap.BeginInit();
                    dpiFixedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    dpiFixedBitmap.StreamSource = stream;
                    dpiFixedBitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    dpiFixedBitmap.EndInit();
                }
                dpiFixedBitmap.Freeze();
                return dpiFixedBitmap;
            }

            return bitmap;
        }
        [RelayCommand] void OpenFullImage(ImageCompositeVM image)
        {
            Image = image.ImageSource;
            IsImagePopupOpen = true;
        }
        [ObservableProperty] bool _isImagePopupOpen;
        [ObservableProperty] BitmapImage _image;

        [ObservableProperty] bool _isOpenPopup;
        [ObservableProperty] bool _isOpenPopupType;
        public ObservableCollection<CommandContextMenu> ContextMenu { get; set; } = new();
        public ObservableCollection<CommandContextMenu> ContextMenuTypes { get; set; } = new();
        [RelayCommand] void OpenPopup(CompositeBaseVM composite)
        {
            ContextMenu.Clear();

            if (composite is TextCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite)); 
                AddMethodDeleteComposite(composite);                  
                AddMethodDuplicateComposite(composite);               
                AddMethodOpenPopupType(composite); 
                IsOpenPopup = true;
                return;
            }
            if (composite is HeaderCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite)); 
                AddMethodDeleteComposite(composite);                  
                AddMethodDuplicateComposite(composite);               
                AddMethodOpenPopupType(composite);                    
                IsOpenPopup = true;
                return;
            }
            if (composite is QuoteCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite)); 
                AddMethodDeleteComposite(composite);                  
                AddMethodDuplicateComposite(composite);               
                AddMethodOpenPopupType(composite);   
                IsOpenPopup = true;
                return;
            }
            if (composite is LineCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite));
                AddMethodDeleteComposite(composite);                 
                AddMethodDuplicateComposite(composite);              
                IsOpenPopup = true;
                return;
            }
            if (composite is TaskCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite));
                AddMethodDeleteComposite(composite);                 
                AddMethodDuplicateComposite(composite);              
                AddMethodOpenPopupType(composite);  
                IsOpenPopup = true;
                return;
            }
            if (composite is ImageCompositeVM)
            {
                AddMethodAddComposite(Composites.IndexOf(composite));
                AddMethodDeleteComposite(composite);                 
                AddMethodDuplicateComposite(composite);              
                IsOpenPopup = true;
                return;
            }

        }

        void CloseContextMenus()
        {
            IsOpenPopupType = false;
            IsOpenPopup = false;
            ContextMenuTypes.Clear();
            ContextMenu.Clear();
        }
        void AddMethodAddComposite(int index)
        {
            ContextMenu.Add(new CommandContextMenu("+ Add", new RelayCommand(() =>
            {
                Composites.Insert(index + 1, new TextCompositeVM());
                IsOpenPopup = false;
                ContextMenu.Clear();
            })));
        }
        void AddMethodDeleteComposite(CompositeBaseVM composite) => ContextMenu.Add(new CommandContextMenu("- Delete", new RelayCommand(() => DeleteComposite(composite))));
        void AddMethodDuplicateComposite(CompositeBaseVM composite) => ContextMenu.Add(new CommandContextMenu("Duplicate", new RelayCommand(() => DuplicateComposite(composite))));
        void AddMethodOpenPopupType(CompositeBaseVM composite) => ContextMenu.Add(new CommandContextMenu("Change type >", new RelayCommand(() =>
        {
            ContextMenuTypes.Clear();
            ContextMenuTypes.Add(new CommandContextMenu("Text", new RelayCommand(() => { ChangeTypeComposite(composite, "Text"); })));
            ContextMenuTypes.Add(new CommandContextMenu("Header1", new RelayCommand(() => { ChangeTypeComposite(composite, "Header1"); })));
            ContextMenuTypes.Add(new CommandContextMenu("Header2", new RelayCommand(() => { ChangeTypeComposite(composite, "Header2"); })));
            ContextMenuTypes.Add(new CommandContextMenu("Header3", new RelayCommand(() => { ChangeTypeComposite(composite, "Header3"); })));
            ContextMenuTypes.Add(new CommandContextMenu("Quote", new RelayCommand(() => { ChangeTypeComposite(composite, "Quote"); })));
            ContextMenuTypes.Add(new CommandContextMenu("Task", new RelayCommand(() => { ChangeTypeComposite(composite, "Task"); })));
            IsOpenPopupType = true;
        })));

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
                    Composites.Clear();
                    ContextMenu.Clear();
                    ContextMenuTypes.Clear();
                    Image = null;
                    IsImagePopupOpen = false;
                    IsOpenPopup = false;
                    IsOpenPopupType = false;
                }
                _disposed = true;
            }
        }
    }
}