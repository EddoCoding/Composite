using CommunityToolkit.Mvvm.Messaging;
using Composite.Models;
using Composite.Models.Notes.HardNote;
using Composite.Services.TabService;
using Composite.Services;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace Composite.Common.Mappers
{
    public class HardNoteMap(ITabService tabService, INoteService noteService, IHardNoteService hardNoteService, IMessenger messenger) : IHardNoteMap
    {
        public HardNote MapToModel(HardNoteVM hardNoteVM)
        {
            List<CompositeBase> composites;
            if (hardNoteVM.Composites.Any())
            {
                composites = new List<CompositeBase>();
                foreach (var compositeVM in hardNoteVM.Composites)
                {
                    var composite = GetComposite(hardNoteVM.Id, compositeVM);
                    composites.Add(composite);
                }
            }
            else composites = new List<CompositeBase>();

            return new HardNote()
            {
                Id = hardNoteVM.Id.ToString(),
                Title = hardNoteVM.Title,
                Category = hardNoteVM.Category,
                DateCreate = hardNoteVM.DateCreate,
                Password = hardNoteVM.Password,
                Composites = composites
            };
        }
        public HardNote MapToModelWithNewIdComposite(HardNoteVM hardNoteVM)
        {
            List<CompositeBase> composites;
            if (hardNoteVM.Composites.Any())
            {
                composites = new List<CompositeBase>();
                foreach (var compositeVM in hardNoteVM.Composites)
                {
                    var composite = GetCompositeNewId(hardNoteVM.Id, compositeVM);
                    composites.Add(composite);
                }
            }
            else composites = new List<CompositeBase>();

            return new HardNote()
            {
                Id = hardNoteVM.Id.ToString(),
                Title = hardNoteVM.Title,
                Category = hardNoteVM.Category,
                DateCreate = hardNoteVM.DateCreate,
                Password = hardNoteVM.Password,
                Composites = composites
            };
        }
        public HardNoteVM MapToViewModel(HardNote hardNote)
        {
            List<CompositeBaseVM> compositesVM;
            if (hardNote.Composites.Any())
            {
                compositesVM = new List<CompositeBaseVM>();
                foreach (var composite in hardNote.Composites)
                {
                    var compositeVM = GetCompositeVM(composite);
                    compositesVM.Add(compositeVM);
                }
            }
            else compositesVM = new List<CompositeBaseVM>();

            return new HardNoteVM(tabService, noteService, hardNoteService, messenger)
            {
                Id = Guid.Parse(hardNote.Id),
                Title = hardNote.Title,
                Category= hardNote.Category,
                DateCreate = hardNote.DateCreate,
                Password = hardNote.Password,
                Composites = new ObservableCollection<CompositeBaseVM>(compositesVM)
            };
        }
        public NoteIdTitle MapToHardNoteIdTitle(HardNote hardNote) => new NoteIdTitle()
        {
            Id = Guid.Parse(hardNote.Id),
            Title = hardNote.Title
        };

        CompositeBase GetComposite(Guid id, CompositeBaseVM compositeBaseVM)
        {
            if (compositeBaseVM is TextCompositeVM textCompositeVM)
            {
                return new TextComposite()
                {
                    Id = textCompositeVM.Id.ToString(),
                    Tag = textCompositeVM.Tag,
                    Comment = textCompositeVM.Comment,
                    Text = textCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is HeaderCompositeVM headerCompositeVM)
            {
                return new HeaderComposite()
                {
                    Id = headerCompositeVM.Id.ToString(),
                    Tag = headerCompositeVM.Tag,
                    Comment = headerCompositeVM.Comment,
                    Header = headerCompositeVM.Text,
                    FontWeightHeader = headerCompositeVM.FontWeight,
                    FontSizeHeader = headerCompositeVM.FontSize,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is QuoteCompositeVM quoteCompositeVM)
            {
                return new QuoteComposite()
                {
                    Id = quoteCompositeVM.Id.ToString(),
                    Tag = quoteCompositeVM.Tag,
                    Comment = quoteCompositeVM.Comment,
                    Quote = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = lineCompositeVM.Id.ToString(),
                    Tag = lineCompositeVM.Tag,
                    Comment = lineCompositeVM.Comment,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is TaskCompositeVM taskCompositeVM)
            {
                return new TaskComposite()
                {
                    Id = taskCompositeVM.Id.ToString(),
                    Tag = taskCompositeVM.Tag,
                    Comment= taskCompositeVM.Comment,
                    Completed = taskCompositeVM.IsCompleted? 1 : 0,
                    TaskText = taskCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is ImageCompositeVM imageCompositeVM)
            {
                return new ImageComposite()
                {
                    Id = imageCompositeVM.Id.ToString(),
                    Tag = imageCompositeVM.Tag,
                    Comment = imageCompositeVM.Comment,
                    DataImage = BitmapImageToByteArray(imageCompositeVM.ImageSource),
                    HorizontalImage = imageCompositeVM.HorizontalImage,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is RefCompositeVM refCompositeVM)
            {
                return new RefComposite()
                {
                    Id = refCompositeVM.Id.ToString(),
                    Tag = refCompositeVM.Tag,
                    Comment = refCompositeVM.Comment,
                    ValueRef = refCompositeVM.ValueRef,
                    Text = refCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is MarkerCompositeVM markerCompositeVM)
            {
                return new MarkerComposite()
                {
                    Id = markerCompositeVM.Id.ToString(),
                    Tag = markerCompositeVM.Tag,
                    Comment = markerCompositeVM.Comment,
                    Text = markerCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is NumericCompositeVM numericCompositeVM)
            {
                return new NumericComposite()
                {
                    Id = numericCompositeVM.Id.ToString(),
                    Tag = numericCompositeVM.Tag,
                    Comment = numericCompositeVM.Comment,
                    Number = numericCompositeVM.Number,
                    Text = numericCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is CodeCompositeVM codeCompositeVM)
            {
                return new CodeComposite()
                {
                    Id = codeCompositeVM.Id.ToString(),
                    Tag = codeCompositeVM.Tag,
                    Comment = codeCompositeVM.Comment,
                    Text = codeCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }

            return null;
        }
        CompositeBase GetCompositeNewId(Guid id, CompositeBaseVM compositeBaseVM)
        {
            if (compositeBaseVM is TextCompositeVM textCompositeVM)
            {
                return new TextComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = textCompositeVM.Tag,
                    Comment = textCompositeVM.Comment,
                    Text = textCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is HeaderCompositeVM headerCompositeVM)
            {
                return new HeaderComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = headerCompositeVM.Tag,
                    Comment = headerCompositeVM.Comment,
                    Header = headerCompositeVM.Text,
                    FontWeightHeader = headerCompositeVM.FontWeight,
                    FontSizeHeader = headerCompositeVM.FontSize,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is QuoteCompositeVM quoteCompositeVM)
            {
                return new QuoteComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = quoteCompositeVM.Tag,
                    Comment = quoteCompositeVM.Comment,
                    Quote = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = lineCompositeVM.Tag,
                    Comment = lineCompositeVM.Comment,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is TaskCompositeVM taskCompositeVM)
            {
                return new TaskComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = taskCompositeVM.Tag,
                    Comment = taskCompositeVM.Comment,
                    Completed = taskCompositeVM.IsCompleted ? 1 : 0,
                    TaskText = taskCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is ImageCompositeVM imageCompositeVM)
            {
                return new ImageComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = imageCompositeVM.Tag,
                    Comment = imageCompositeVM.Comment,
                    DataImage = BitmapImageToByteArray(imageCompositeVM.ImageSource),
                    HorizontalImage = imageCompositeVM.HorizontalImage,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is RefCompositeVM refCompositeVM)
            {
                return new RefComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = refCompositeVM.Tag,
                    Comment = refCompositeVM.Comment,
                    ValueRef = refCompositeVM.ValueRef,
                    Text = refCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is MarkerCompositeVM markerCompositeVM)
            {
                return new MarkerComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = markerCompositeVM.Tag,
                    Comment = markerCompositeVM.Comment,
                    Text = markerCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is NumericCompositeVM numericCompositeVM)
            {
                return new NumericComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = numericCompositeVM.Tag,
                    Comment = numericCompositeVM.Comment,
                    Number = numericCompositeVM.Number,
                    Text = numericCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is CodeCompositeVM codeCompositeVM)
            {
                return new CodeComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = codeCompositeVM.Tag,
                    Comment = codeCompositeVM.Comment,
                    Text = codeCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }

            return null;
        }
        CompositeBaseVM GetCompositeVM(CompositeBase compositeBase)
        {
            if (compositeBase.CompositeType == "TextComposite")
            {
                return new TextCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Text = compositeBase.Text
                };
            }
            if (compositeBase.CompositeType == "HeaderComposite")
            {
                return new HeaderCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Text = compositeBase.Header,
                    FontWeight = compositeBase.FontWeightHeader,
                    FontSize = compositeBase.FontSizeHeader
                };
            }
            if (compositeBase.CompositeType == "QuoteComposite")
            {
                return new QuoteCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Text = compositeBase.Quote
                };
            }
            if (compositeBase.CompositeType == "LineComposite")
            {
                return new LineCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment
                };
            }
            if (compositeBase.CompositeType == "TaskComposite")
            {
                return new TaskCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    IsCompleted = (compositeBase.Completed == 1) ? true : false,
                    Text = compositeBase.TaskText
                };
            }
            if (compositeBase.CompositeType == "ImageComposite")
            {
                BitmapImage bitmapImage = ByteArrayToBitmapImage(compositeBase.DataImage);

                return new ImageCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    ImageSource = bitmapImage,
                    HorizontalImage = compositeBase.HorizontalImage,
                    OriginalWidth = bitmapImage.PixelWidth,
                    OriginalHeight = bitmapImage.PixelHeight
                };
            }
            if (compositeBase.CompositeType == "RefComposite")
            {
                return new RefCompositeVM(tabService, noteService, hardNoteService, messenger)
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    ValueRef = compositeBase.ValueRef,
                    Text = compositeBase.Text
                };
            }
            if (compositeBase.CompositeType == "MarkerComposite")
            {
                return new MarkerCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Text = compositeBase.Text
                };
            }
            if (compositeBase.CompositeType == "NumericComposite")
            {
                return new NumericCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Number = compositeBase.Number,
                    Text = compositeBase.Text
                };
            }
            if (compositeBase.CompositeType == "CodeComposite")
            {
                return new CodeCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
                    Tag = compositeBase.Tag,
                    Comment = compositeBase.Comment,
                    Text = compositeBase.Text
                };
            }

            return null;
        }

        byte[] BitmapImageToByteArray(BitmapImage bitmapImage)
        {
            if (bitmapImage == null) return null;

            byte[] data;

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }
        BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            BitmapImage image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
            }

            return image;
        }
    }
}