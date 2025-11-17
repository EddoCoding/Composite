using CommunityToolkit.Mvvm.Messaging;
using Composite.Services;
using Composite.Services.TabService;
using Composite.ViewModels.Notes.HardNote;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Composite.Common.Factories
{
    public class HardNoteFactory(ITabService tabService, IHardNoteService hardNoteService, IMessenger messenger) : IHardNoteFactory
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
                        Comment = lineCompositeVM.Comment,
                        SelectedLineSize = lineCompositeVM.LineSize,
                        SelectedLineColor = lineCompositeVM.LineColor
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
                    var newRefCompositeVM = new RefCompositeVM(tabService, hardNoteService, messenger)
                    {
                        Id = refCompositeVM.Id,
                        Tag = refCompositeVM.Tag,
                        Comment = refCompositeVM.Comment,
                        ValueRef = refCompositeVM.ValueRef,
                        Text = refCompositeVM.Text
                    };
                    compositesVM.Add(newRefCompositeVM);
                }
                else if (compositeVM is MarkerCompositeVM markerCompositeVM)
                {
                    var newMarkerCompositeVM = new MarkerCompositeVM()
                    {
                        Id = markerCompositeVM.Id,
                        Tag = markerCompositeVM.Tag,
                        Comment = markerCompositeVM.Comment,
                        Text = markerCompositeVM.Text
                    };
                    compositesVM.Add(newMarkerCompositeVM);
                }
                else if (compositeVM is NumericCompositeVM numericCompositeVM)
                {
                    var newNumericCompositeVM = new NumericCompositeVM()
                    {
                        Id = numericCompositeVM.Id,
                        Tag = numericCompositeVM.Tag,
                        Comment = numericCompositeVM.Comment,
                        Number = numericCompositeVM.Number,
                        Text = numericCompositeVM.Text
                    };
                    compositesVM.Add(newNumericCompositeVM);
                }
                else if (compositeVM is CodeCompositeVM codeCompositeVM)
                {
                    var newCodeCompositeVM = new CodeCompositeVM()
                    {
                        Id = codeCompositeVM.Id,
                        Tag = codeCompositeVM.Tag,
                        Comment = codeCompositeVM.Comment,
                        Text = codeCompositeVM.Text
                    };
                    compositesVM.Add(newCodeCompositeVM);
                }
                else if (compositeVM is DocCompositeVM docCompositeVM)
                {
                    var newDocCompositeVM = new DocCompositeVM(hardNoteService)
                    {
                        Id = docCompositeVM.Id,
                        Tag = docCompositeVM.Tag,
                        Text = docCompositeVM.Text,
                        Comment = docCompositeVM.Comment,
                        Data = docCompositeVM.Data
                    };
                    compositesVM.Add(newDocCompositeVM);
                }
                else if (compositeVM is FormattedTextCompositeVM ftCompositeVM)
                {
                    var document = new FlowDocument();
                    TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

                    using (MemoryStream stream = new MemoryStream(ftCompositeVM.XamlPackageContent))
                    {
                        range.Load(stream, DataFormats.XamlPackage);
                    }

                    var newFTCompositeVM = new FormattedTextCompositeVM()
                    {
                        Id = ftCompositeVM.Id,
                        Tag = ftCompositeVM.Tag,
                        Comment = ftCompositeVM.Comment,
                        Document = document,
                        SelectedBrSize = ftCompositeVM.BrSize,
                        SelectedBrCornerRadius = ftCompositeVM.BrCornerRadius,
                        SelectedBrColor = ftCompositeVM.BrColor,
                        SelectedBgColor = ftCompositeVM.BgColor,
                        XamlPackageContent = ftCompositeVM.XamlPackageContent
                    };

                    compositesVM.Add(newFTCompositeVM);
                }
            }

            return new HardNoteVM(tabService, hardNoteService, messenger)
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