using CommunityToolkit.Mvvm.Messaging;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Factories
{
    public class HardNoteFactory(ITabService tabService, INoteService noteService, IHardNoteService hardNoteService, IMessenger messenger) : IHardNoteFactory
    {
        public HardNoteVM CreateHardNoteVM(HardNoteVM hardNoteVM)
        {
            List<CompositeBaseVM> compositesVM = new();
            foreach(var compositeVM in hardNoteVM.Composites)
            {
                if (compositeVM is TextCompositeVM textCompositeVM)
                {
                    var newTextCompositeVM = new TextCompositeVM()
                    {
                        Id = textCompositeVM.Id,
                        Tag = textCompositeVM.Tag,
                        Comment = textCompositeVM.Comment,
                        Text = textCompositeVM.Text
                    };
                    compositesVM.Add(newTextCompositeVM);
                }
                else if (compositeVM is HeaderCompositeVM headerCompositeVM)
                {
                    var newHeaderCompositeVM = new HeaderCompositeVM()
                    {
                        Id = headerCompositeVM.Id,
                        Tag = headerCompositeVM.Tag,
                        Comment = headerCompositeVM.Comment,
                        Text = headerCompositeVM.Text,
                        FontWeight = headerCompositeVM.FontWeight,
                        FontSize = headerCompositeVM.FontSize
                    };
                    compositesVM.Add(newHeaderCompositeVM);
                }
                else if (compositeVM is QuoteCompositeVM quoteCompositeVM)
                {
                    var newQuoteCompositeVM = new QuoteCompositeVM()
                    {
                        Id = quoteCompositeVM.Id,
                        Tag = quoteCompositeVM.Tag,
                        Comment = quoteCompositeVM.Comment,
                        Text = quoteCompositeVM.Text
                    };
                    compositesVM.Add(newQuoteCompositeVM);
                }
                else if (compositeVM is LineCompositeVM lineCompositeVM)
                {
                    var newLineCompositeVM = new LineCompositeVM()
                    {
                        Id = lineCompositeVM.Id,
                        Tag = lineCompositeVM.Tag,
                        Comment = lineCompositeVM.Comment
                    };
                    compositesVM.Add(newLineCompositeVM);
                }
                else if (compositeVM is TaskCompositeVM taskCompositeVM)
                {
                    var newTaskCompositeVM = new TaskCompositeVM()
                    {
                        Id = taskCompositeVM.Id,
                        Tag = taskCompositeVM.Tag,
                        Comment = taskCompositeVM.Comment,
                        Text = taskCompositeVM.Text,
                        IsCompleted = taskCompositeVM.IsCompleted
                    };
                    compositesVM.Add(newTaskCompositeVM);
                }
                else if (compositeVM is ImageCompositeVM imageCompositeVM)
                {
                    var newImageCompositeVM = new ImageCompositeVM()
                    {
                        Id = imageCompositeVM.Id,
                        Tag = imageCompositeVM.Tag,
                        Comment = imageCompositeVM.Comment,
                        ImageSource = imageCompositeVM.ImageSource,
                        OriginalWidth = imageCompositeVM.OriginalWidth,
                        OriginalHeight = imageCompositeVM.OriginalHeight,
                        HorizontalImage = imageCompositeVM.HorizontalImage
                    };
                    compositesVM.Add(newImageCompositeVM);
                }
                else if (compositeVM is RefCompositeVM refCompositeVM)
                {
                    var newRefCompositeVM = new RefCompositeVM(tabService, noteService, hardNoteService, messenger)
                    {
                        Id = refCompositeVM.Id,
                        Tag = refCompositeVM.Tag,
                        Comment = refCompositeVM.Comment,
                        ValueRef = refCompositeVM.ValueRef,
                        Text = refCompositeVM.Text
                    };
                    compositesVM.Add(newRefCompositeVM);
                }
            }

            return new HardNoteVM(tabService, noteService, hardNoteService, messenger)
            {
                Id = hardNoteVM.Id,
                Title = hardNoteVM.Title,
                Category = hardNoteVM.Category,
                Password = hardNoteVM.Password,
                Composites = new(compositesVM)
            };
        }
    }
}