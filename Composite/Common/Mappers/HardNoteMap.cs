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
                    Tag = quoteCompositeVM.Tag,
                    Comment = quoteCompositeVM.Comment,
                    Text = quoteCompositeVM.Text,
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
                    Tag = taskCompositeVM.Tag,
                    Comment = taskCompositeVM.Comment,
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
                    Tag = imageCompositeVM.Tag,
                    Comment = imageCompositeVM.Comment,
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
            if (compositeBaseVM is DocCompositeVM docCompositeVM)
            {
                return new DocComposite()
                {
                    Id = docCompositeVM.Id.ToString(),
                    Tag = docCompositeVM.Tag,
                    Comment = docCompositeVM.Comment,
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
                    Tag = ftCompositeVM.Tag,
                    Comment = ftCompositeVM.Comment,
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
                    Tag = songCompositeVM.Tag,
                    Comment = songCompositeVM.Comment,
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
                    Tag = refListCompositeVM.Tag,
                    Comment = refListCompositeVM.Comment,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var referenceVM in refListCompositeVM.References)
                {
                    var refComposite = new RefComposite()
                    {
                        Id = referenceVM.Id.ToString(),
                        Tag = referenceVM.Tag,
                        Comment = referenceVM.Comment,
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
                    Tag = taskListCompositeVM.Tag,
                    Comment = taskListCompositeVM.Comment,
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
                        Tag = subTaskVM.Tag,
                        Comment = subTaskVM.Comment,
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
                    Tag = docListCompositeVM.Tag,
                    Comment = docListCompositeVM.Comment,
                    Text = docListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var documentVM in docListCompositeVM.Documents)
                {
                    var documentComposite = new DocumentComposite()
                    {
                        Id = documentVM.Id.ToString(),
                        Tag = documentVM.Tag,
                        Comment = documentVM.Comment,
                        Text = documentVM.Text,
                        Data = documentVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = docListComposite.Id
                    };

                    docListComposite.Children.Add(documentComposite);
                }

                return docListComposite;
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
                    Tag = quoteCompositeVM.Tag,
                    Comment = quoteCompositeVM.Comment,
                    Text = quoteCompositeVM.Text,
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
                    Tag = taskCompositeVM.Tag,
                    Comment = taskCompositeVM.Comment,
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
                    Tag = imageCompositeVM.Tag,
                    Comment = imageCompositeVM.Comment,
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
            if (compositeBaseVM is DocCompositeVM docCompositeVM)
            {
                return new DocComposite()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tag = docCompositeVM.Tag,
                    Comment = docCompositeVM.Comment,
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
                    Tag = ftCompositeVM.Tag,
                    Comment = ftCompositeVM.Comment,
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
                    Tag = songCompositeVM.Tag,
                    Comment = songCompositeVM.Comment,
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
                    Tag = refListCompositeVM.Tag,
                    Comment = refListCompositeVM.Comment,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var referenceVM in refListCompositeVM.References)
                {
                    var refComposite = new RefComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Tag = referenceVM.Tag,
                        Comment = referenceVM.Comment,
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
                    Tag = taskListCompositeVM.Tag,
                    Comment = taskListCompositeVM.Comment,
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
                        Tag = subTaskVM.Tag,
                        Comment = subTaskVM.Comment,
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
                    Tag = docListCompositeVM.Tag,
                    Comment = docListCompositeVM.Comment,
                    Text = docListCompositeVM.Text,
                    HardNoteId = id.ToString(),
                    Children = new List<CompositeBase>()
                };

                foreach (var documentVM in docListCompositeVM.Documents)
                {
                    var documentComposite = new DocumentComposite()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Tag = documentVM.Tag,
                        Comment = documentVM.Comment,
                        Text = documentVM.Text,
                        Data = documentVM.Data,
                        HardNoteId = id.ToString(),
                        ParentId = docListComposite.Id
                    };

                    docListComposite.Children.Add(documentComposite);
                }

                return docListComposite;
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
                        Tag = textComposite.Tag,
                        Comment = textComposite.Comment,
                        Text = textComposite.Text
                    };
                case HeaderComposite headerComposite:
                    return new HeaderCompositeVM()
                    {
                        Id = Guid.Parse(headerComposite.Id),
                        Tag = headerComposite.Tag,
                        Comment = headerComposite.Comment,
                        Text = headerComposite.Text,
                        FontWeight = headerComposite.FontWeight,
                        FontSize = headerComposite.FontSize
                    };
                case QuoteComposite quoteComposite:
                    return new QuoteCompositeVM()
                    {
                        Id = Guid.Parse(quoteComposite.Id),
                        Tag = quoteComposite.Tag,
                        Comment = quoteComposite.Comment,
                        Text = quoteComposite.Text
                    };
                case LineComposite lineComposite:
                    return new LineCompositeVM()
                    {
                        Id = Guid.Parse(lineComposite.Id),
                        Tag = lineComposite.Tag,
                        SelectedLineSize = lineComposite.LineSize,
                        SelectedLineColor = lineComposite.LineColor,
                        Comment = lineComposite.Comment
                    };
                case TaskComposite taskComposite:
                    return new TaskCompositeVM()
                    {
                        Id = Guid.Parse(taskComposite.Id),
                        Tag = taskComposite.Tag,
                        Comment = taskComposite.Comment,
                        IsCompleted = taskComposite.Completed == 1,
                        Text = taskComposite.Text
                    };
                case ImageComposite imageComposite:
                    {
                        BitmapImage bitmapImage = ByteArrayToBitmapImage(imageComposite.Data);
                        return new ImageCompositeVM()
                        {
                            Id = Guid.Parse(imageComposite.Id),
                            Tag = imageComposite.Tag,
                            Comment = imageComposite.Comment,
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
                        Tag = referenceComposite.Tag,
                        Comment = referenceComposite.Comment,
                        ValueRef = referenceComposite.ValueRef,
                        Text = referenceComposite.Text
                    };
                case MarkerComposite markerComposite:
                    return new MarkerCompositeVM()
                    {
                        Id = Guid.Parse(markerComposite.Id),
                        Tag = markerComposite.Tag,
                        Comment = markerComposite.Comment,
                        Text = markerComposite.Text
                    };
                case NumericComposite numericComposite:
                    return new NumericCompositeVM()
                    {
                        Id = Guid.Parse(numericComposite.Id),
                        Tag = numericComposite.Tag,
                        Comment = numericComposite.Comment,
                        Number = numericComposite.Number,
                        Text = numericComposite.Text
                    };
                case CodeComposite codeComposite:
                    return new CodeCompositeVM()
                    {
                        Id = Guid.Parse(codeComposite.Id),
                        Tag = codeComposite.Tag,
                        Comment = codeComposite.Comment,
                        Text = codeComposite.Text
                    };
                case DocComposite documentComposite:
                    return new DocCompositeVM(hardNoteService)
                    {
                        Id = Guid.Parse(documentComposite.Id),
                        Tag = documentComposite.Tag,
                        Comment = documentComposite.Comment,
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
                        Tag = formattedTextComposite.Tag,
                        Comment = formattedTextComposite.Comment,
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
                        Tag = songComposite.Tag,
                        Comment = songComposite.Comment,
                        Title = songComposite.Title,
                        Data = songComposite.Data
                    };

                case RefListComposite refListComposite:
                    var refListVM = new RefListCompositeVM(tabService, hardNoteService, messenger)
                    {
                        Id = Guid.Parse(refListComposite.Id),
                        Tag = refListComposite.Tag,
                        Comment = refListComposite.Comment
                    };

                    foreach (var child in refListComposite.Children.OfType<RefComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var refVM = new ReferenceCompositeVM(tabService, hardNoteService, messenger)
                        {
                            Id = Guid.Parse(child.Id),
                            Tag = child.Tag,
                            Comment = child.Comment,
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
                        Tag = taskListComposite.Tag,
                        Comment = taskListComposite.Comment,
                        Text = taskListComposite.Text,
                        Status = taskListComposite.Status,
                        IsCompleted = taskListComposite.Completed == 1
                    };

                    foreach (var child in taskListComposite.Children.OfType<SubTaskComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var subTaskVM = new SubTaskCompositeVM(taskListVM.CalculatingPercentTask)
                        {
                            Id = Guid.Parse(child.Id),
                            Tag = child.Tag,
                            Comment = child.Comment,
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
                        Tag = docListComposite.Tag,
                        Comment = docListComposite.Comment,
                        Text = docListComposite.Text
                    };

                    foreach (var child in docListComposite.Children.OfType<DocumentComposite>().OrderBy(c => c.OrderIndex))
                    {
                        var documentVM = new DocumentCompositeVM(hardNoteService)
                        {
                            Id = Guid.Parse(child.Id),
                            Tag = child.Tag,
                            Comment = child.Comment,
                            Text = child.Text,
                            Data = child.Data
                        };

                        docListVM.Documents.Add(documentVM);
                    }

                    return docListVM;

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