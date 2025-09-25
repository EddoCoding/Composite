using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Factories
{
    public class HardNoteFactory : IHardNoteFactory
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
                        Text = textCompositeVM.Text
                    };
                    compositesVM.Add(newTextCompositeVM);
                }
                else if (compositeVM is HeaderCompositeVM headerCompositeVM)
                {
                    var newHeaderCompositeVM = new HeaderCompositeVM()
                    {
                        Id = headerCompositeVM.Id,
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
                        Text = quoteCompositeVM.Text
                    };
                    compositesVM.Add(newQuoteCompositeVM);
                }
                else if (compositeVM is LineCompositeVM lineCompositeVM)
                {
                    var newLineCompositeVM = new LineCompositeVM()
                    {
                        Id = lineCompositeVM.Id
                    };
                    compositesVM.Add(newLineCompositeVM);
                }
                else if (compositeVM is TaskCompositeVM taskCompositeVM)
                {
                    var newTaskCompositeVM = new TaskCompositeVM()
                    {
                        Id = taskCompositeVM.Id,
                        Text = taskCompositeVM.Text,
                        IsCompleted = taskCompositeVM.IsCompleted
                    };
                    compositesVM.Add(newTaskCompositeVM);
                }
            }

            return new HardNoteVM()
            {
                Id = hardNoteVM.Id,
                Title = hardNoteVM.Title,
                Category = hardNoteVM.Category,
                Composites = new(compositesVM)
            };
        }
    }
}