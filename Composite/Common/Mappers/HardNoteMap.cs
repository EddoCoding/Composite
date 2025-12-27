using CommunityToolkit.Mvvm.Messaging;
using Composite.Models;
using Composite.Models.Notes.HardNote;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes;
using Composite.ViewModels.Notes.HardNote;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Composite.Common.Mappers
{
    public class HardNoteMap(ITabService tabService, IHardNoteService hardNoteService, IMessenger messenger) : IHardNoteMap
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

            return new HardNoteVM(tabService, hardNoteService, messenger)
            {
                Id = Guid.Parse(hardNote.Id),
                Title = hardNote.Title,
                Category = hardNote.Category,
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
                    Text = textCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is HeaderCompositeVM headerCompositeVM)
            {
                return new HeaderComposite()
                {
                    Id = headerCompositeVM.Id.ToString(),
                    Text = headerCompositeVM.Text,
                    FontWeight = headerCompositeVM.FontWeight,
                    FontSize = (int)headerCompositeVM.FontSize,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is QuoteCompositeVM quoteCompositeVM)
            {
                return new QuoteComposite()
                {
                    Id = quoteCompositeVM.Id.ToString(),
                    Text = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = lineCompositeVM.Id.ToString(),
                    LineSize = (int)lineCompositeVM.SelectedLineSize,
                    LineColor = lineCompositeVM.SelectedLineColor,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is TaskCompositeVM taskCompositeVM)
            {
                return new TaskComposite()
                {
                    Id = taskCompositeVM.Id.ToString(),
                    Completed = taskCompositeVM.IsCompleted ? 1 : 0,
                    Text = taskCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is ImageCompositeVM imageCompositeVM)
            {
                return new ImageComposite()
                {
                    Id = imageCompositeVM.Id.ToString(),
                    Data = BitmapImageToByteArray(imageCompositeVM.ImageSource),
                    HorizontalAlignment = imageCompositeVM.HorizontalImage,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is RefCompositeVM refCompositeVM)
            {
                return new RefComposite()
                {
                    Id = refCompositeVM.Id.ToString(),
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
                    Text = markerCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is NumericCompositeVM numericCompositeVM)
            {
                return new NumericComposite()
                {
                    Id = numericCompositeVM.Id.ToString(),
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
                    Text = codeCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is DocCompositeVM docCompositeVM)
            {
                return new DocComposite()
                {
                    Id = docCompositeVM.Id.ToString(),
                    Text = docCompositeVM.Text,
                    Data = docCompositeVM.Data,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is FormattedTextCompositeVM ftCompositeVM)
            {
                return new FormattedTextComposite()
                {
                    Id = ftCompositeVM.Id.ToString(),
                    Data = ftCompositeVM.XamlPackageContent,
                    BorderSize = (int)ftCompositeVM.SelectedBrSize,
                    CornerRadius = (int)ftCompositeVM.SelectedBrCornerRadius,
                    BorderColor = ftCompositeVM.SelectedBrColor,
                    BackgroundColor = ftCompositeVM.SelectedBgColor,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is SongCompositeVM songCompositeVM)
            {
                return new SongComposite()
                {
                    Id = songCompositeVM.Id.ToString(),
                    Title = songCompositeVM.Title,
                    Data = songCompositeVM.Data,
                    HardNoteId = id.ToString()
                };
            }

            if (compositeBaseVM is RefListCompositeVM refListCompositeVM)
            {
                var refListComposite = new RefListComposite()
                {
                    Id = refListCompositeVM.Id.ToString(),
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var referenceVM in refListCompositeVM.References)
                {
                    var refComposite = new RefComposite()
                    {
                        Id = referenceVM.Id.ToString(),
                        Text = referenceVM.Text,
                        ValueRef = referenceVM.ValueRef,
                        HardNoteId = id.ToString(),
                        ParentId = refListComposite.Id
                    };

                    refListComposite.Children.Add(refComposite);
                }

                return refListComposite;
            }
            if (compositeBaseVM is TaskListCompositeVM taskListCompositeVM)
            {
                var taskListComposite = new TaskListComposite()
                {
                    Id = taskListCompositeVM.Id.ToString(),
                    Text = taskListCompositeVM.Text,
                    Status = taskListCompositeVM.Status,
                    Completed = taskListCompositeVM.IsCompleted ? 1 : 0,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var subTaskVM in taskListCompositeVM.SubTasks)
                {
                    var subTaskComposite = new SubTaskComposite()
                    {
                        Id = subTaskVM.Id.ToString(),
                        Text = subTaskVM.Text,
                        Completed = subTaskVM.IsCompleted ? 1 : 0,
                        HardNoteId = id.ToString(),
                        ParentId = taskListComposite.Id
                    };

                    taskListComposite.Children.Add(subTaskComposite);
                }

                return taskListComposite;
            }
            if (compositeBaseVM is DocListCompositeVM docListCompositeVM)
            {
                var docListComposite = new DocListComposite()
                {
                    Id = docListCompositeVM.Id.ToString(),
                    Text = docListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var documentVM in docListCompositeVM.Documents)
                {
                    var documentComposite = new DocumentComposite()
                    {
                        Id = documentVM.Id.ToString(),
                        Text = documentVM.Text,
                        Data = documentVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = docListComposite.Id
                    };

                    docListComposite.Children.Add(documentComposite);
                }

                return docListComposite;
            }
            if (compositeBaseVM is SongListCompositeVM songListCompositeVM)
            {
                var songListComposite = new SongListComposite()
                {
                    Id = songListCompositeVM.Id.ToString(),
                    Text = songListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var songVM in songListCompositeVM.Songs)
                {
                    var songComposite = new SongComposite()
                    {
                        Id = songVM.Id.ToString(),
                        Title = songVM.Title,
                        Data = songVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = songListComposite.Id
                    };

                    songListComposite.Children.Add(songComposite);
                }

                return songListComposite;
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
                    Text = textCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is HeaderCompositeVM headerCompositeVM)
            {
                return new HeaderComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = headerCompositeVM.Text,
                    FontWeight = headerCompositeVM.FontWeight,
                    FontSize = (int)headerCompositeVM.FontSize,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is QuoteCompositeVM quoteCompositeVM)
            {
                return new QuoteComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    LineSize = (int)lineCompositeVM.SelectedLineSize,
                    LineColor = lineCompositeVM.SelectedLineColor,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is TaskCompositeVM taskCompositeVM)
            {
                return new TaskComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Completed = taskCompositeVM.IsCompleted ? 1 : 0,
                    Text = taskCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is ImageCompositeVM imageCompositeVM)
            {
                return new ImageComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Data = BitmapImageToByteArray(imageCompositeVM.ImageSource),
                    HorizontalAlignment = imageCompositeVM.HorizontalImage,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is RefCompositeVM refCompositeVM)
            {
                return new RefComposite()
                {
                    Id = Guid.NewGuid().ToString(),
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
                    Text = markerCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is NumericCompositeVM numericCompositeVM)
            {
                return new NumericComposite()
                {
                    Id = Guid.NewGuid().ToString(),
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
                    Text = codeCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is DocCompositeVM docCompositeVM)
            {
                return new DocComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = docCompositeVM.Text,
                    Data = docCompositeVM.Data,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is FormattedTextCompositeVM ftCompositeVM)
            {
                return new FormattedTextComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Data = ftCompositeVM.XamlPackageContent,
                    BorderSize = (int)ftCompositeVM.SelectedBrSize,
                    CornerRadius = (int)ftCompositeVM.SelectedBrCornerRadius,
                    BorderColor = ftCompositeVM.SelectedBrColor,
                    BackgroundColor = ftCompositeVM.SelectedBgColor,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is SongCompositeVM songCompositeVM)
            {
                return new SongComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = songCompositeVM.Title,
                    Data = songCompositeVM.Data,
                    HardNoteId = id.ToString()
                };
            }

            if (compositeBaseVM is RefListCompositeVM refListCompositeVM)
            {
                var refListComposite = new RefListComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var referenceVM in refListCompositeVM.References)
                {
                    var refComposite = new RefComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = referenceVM.Text,
                        ValueRef = referenceVM.ValueRef,
                        HardNoteId = id.ToString(),
                        ParentId = refListComposite.Id
                    };

                    refListComposite.Children.Add(refComposite);
                }

                return refListComposite;
            }
            if (compositeBaseVM is TaskListCompositeVM taskListCompositeVM)
            {
                var taskListComposite = new TaskListComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = taskListCompositeVM.Text,
                    Status = taskListCompositeVM.Status,
                    Completed = taskListCompositeVM.IsCompleted ? 1 : 0,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var subTaskVM in taskListCompositeVM.SubTasks)
                {
                    var subTaskComposite = new SubTaskComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = subTaskVM.Text,
                        Completed = subTaskVM.IsCompleted ? 1 : 0,
                        HardNoteId = id.ToString(),
                        ParentId = taskListComposite.Id
                    };

                    taskListComposite.Children.Add(subTaskComposite);
                }

                return taskListComposite;
            }
            if (compositeBaseVM is DocListCompositeVM docListCompositeVM)
            {
                var docListComposite = new DocListComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = docListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var documentVM in docListCompositeVM.Documents)
                {
                    var documentComposite = new DocumentComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = documentVM.Text,
                        Data = documentVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = docListComposite.Id
                    };

                    docListComposite.Children.Add(documentComposite);
                }

                return docListComposite;
            }
            if (compositeBaseVM is SongListCompositeVM songListCompositeVM)
            {
                var songListComposite = new SongListComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = songListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var songVM in songListCompositeVM.Songs)
                {
                    var songComposite = new SongComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = songVM.Title,
                        Data = songVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = songListComposite.Id
                    };

                    songListComposite.Children.Add(songComposite);
                }

                return songListComposite;
            }

            return null;
        }
        CompositeBaseVM GetCompositeVM(CompositeBase compositeBase)
        {
            switch (compositeBase)
            {
                case TextComposite textComposite:
                    return new TextCompositeVM()
                    {
                        Id = Guid.Parse(textComposite.Id),
                        Text = textComposite.Text
                    };
                case HeaderComposite headerComposite:
                    return new HeaderCompositeVM()
                    {
                        Id = Guid.Parse(headerComposite.Id),
                        Text = headerComposite.Text,
                        FontWeight = headerComposite.FontWeight,
                        FontSize = headerComposite.FontSize
                    };
                case QuoteComposite quoteComposite:
                    return new QuoteCompositeVM()
                    {
                        Id = Guid.Parse(quoteComposite.Id),
                        Text = quoteComposite.Text
                    };
                case LineComposite lineComposite:
                    return new LineCompositeVM()
                    {
                        Id = Guid.Parse(lineComposite.Id),
                        SelectedLineSize = lineComposite.LineSize,
                        SelectedLineColor = lineComposite.LineColor
                    };
                case TaskComposite taskComposite:
                    return new TaskCompositeVM()
                    {
                        Id = Guid.Parse(taskComposite.Id),
                        IsCompleted = taskComposite.Completed == 1,
                        Text = taskComposite.Text
                    };
                case ImageComposite imageComposite:
                    {
                        BitmapImage bitmapImage = ByteArrayToBitmapImage(imageComposite.Data);
                        return new ImageCompositeVM()
                        {
                            Id = Guid.Parse(imageComposite.Id),
                            ImageSource = bitmapImage,
                            HorizontalImage = imageComposite.HorizontalAlignment,
                            OriginalWidth = bitmapImage.PixelWidth,
                            OriginalHeight = bitmapImage.PixelHeight
                        };
                    }
                case RefComposite referenceComposite:
                    return new RefCompositeVM(tabService, hardNoteService, messenger)
                    {
                        Id = Guid.Parse(referenceComposite.Id),
                        ValueRef = referenceComposite.ValueRef,
                        Text = referenceComposite.Text
                    };
                case MarkerComposite markerComposite:
                    return new MarkerCompositeVM()
                    {
                        Id = Guid.Parse(markerComposite.Id),
                        Text = markerComposite.Text
                    };
                case NumericComposite numericComposite:
                    return new NumericCompositeVM()
                    {
                        Id = Guid.Parse(numericComposite.Id),
                        Number = numericComposite.Number,
                        Text = numericComposite.Text
                    };
                case CodeComposite codeComposite:
                    return new CodeCompositeVM()
                    {
                        Id = Guid.Parse(codeComposite.Id),
                        Text = codeComposite.Text
                    };
                case DocComposite documentComposite:
                    return new DocCompositeVM(hardNoteService)
                    {
                        Id = Guid.Parse(documentComposite.Id),
                        Text = documentComposite.Text,
                        Data = documentComposite.Data
                    };
                case FormattedTextComposite formattedTextComposite:
                    var document = new FlowDocument();
                    TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

                    using (MemoryStream stream = new MemoryStream(formattedTextComposite.Data))
                    {
                        range.Load(stream, DataFormats.XamlPackage);
                    }

                    return new FormattedTextCompositeVM()
                    {
                        Id = Guid.Parse(formattedTextComposite.Id),
                        Document = document,
                        SelectedBrSize = formattedTextComposite.BorderSize,
                        SelectedBrCornerRadius = formattedTextComposite.CornerRadius,
                        SelectedBrColor = formattedTextComposite.BorderColor,
                        SelectedBgColor = formattedTextComposite.BackgroundColor,
                        XamlPackageContent = formattedTextComposite.Data,
                        IsModified = false
                    };
                case SongComposite songComposite:
                    return new SongCompositeVM()
                    {
                        Id = Guid.Parse(songComposite.Id),
                        Title = songComposite.Title,
                        Data = songComposite.Data
                    };

                case RefListComposite refListComposite:
                    var refListVM = new RefListCompositeVM(tabService, hardNoteService, messenger)
                    {
                        Id = Guid.Parse(refListComposite.Id)
                    };

                    foreach (var child in refListComposite.Children.OfType<RefComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var refVM = new ReferenceCompositeVM(tabService, hardNoteService, messenger)
                        {
                            Id = Guid.Parse(child.Id),
                            Text = child.Text,
                            ValueRef = child.ValueRef
                        };

                        refListVM.References.Add(refVM);
                    }

                    return refListVM;
                case TaskListComposite taskListComposite:
                    var taskListVM = new TaskListCompositeVM()
                    {
                        Id = Guid.Parse(taskListComposite.Id),
                        Text = taskListComposite.Text,
                        Status = taskListComposite.Status,
                        IsCompleted = taskListComposite.Completed == 1
                    };

                    foreach (var child in taskListComposite.Children.OfType<SubTaskComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var subTaskVM = new SubTaskCompositeVM(taskListVM.CalculatingPercentTask)
                        {
                            Id = Guid.Parse(child.Id),
                            Text = child.Text,
                            IsCompleted = child.Completed == 1
                        };

                        taskListVM.SubTasks.Add(subTaskVM);
                    }

                    return taskListVM;
                case DocListComposite docListComposite:
                    var docListVM = new DocListCompositeVM(hardNoteService)
                    {
                        Id = Guid.Parse(docListComposite.Id),
                        Text = docListComposite.Text
                    };

                    foreach (var child in docListComposite.Children.OfType<DocumentComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var documentVM = new DocumentCompositeVM(hardNoteService)
                        {
                            Id = Guid.Parse(child.Id),
                            Text = child.Text,
                            Data = child.Data
                        };

                        docListVM.Documents.Add(documentVM);
                    }

                    return docListVM;
                case SongListComposite songListComposite:
                    var songListVM = new SongListCompositeVM()
                    {
                        Id = Guid.Parse(songListComposite.Id),
                        Text = songListComposite.Text
                    };

                    foreach (var child in songListComposite.Children.OfType<SongComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var songVM = new SongMiniCompositeVM()
                        {
                            Id = Guid.Parse(child.Id),
                            Title = child.Title,
                            Data = child.Data
                        };

                        songListVM.Songs.Add(songVM);
                    }

                    return songListVM;

                default:
                    return null;
            }
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