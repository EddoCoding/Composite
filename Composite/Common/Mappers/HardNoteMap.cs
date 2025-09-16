using System.Collections.ObjectModel;
using Composite.Models;
using Composite.Models.Notes.HardNote;
using Composite.ViewModels.Notes.HardNote;

namespace Composite.Common.Mappers
{
    public class HardNoteMap : IHardNoteMap
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

            return new HardNoteVM()
            {
                Id = Guid.Parse(hardNote.Id),
                Title = hardNote.Title,
                Category = hardNote.Category,
                Composites = new ObservableCollection<CompositeBaseVM>(compositesVM)
            };
        }

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
                    Quote = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = lineCompositeVM.Id.ToString(),
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
                    Text = textCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is HeaderCompositeVM headerCompositeVM)
            {
                return new HeaderComposite()
                {
                    Id = headerCompositeVM.Id.ToString(),
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
                    Quote = quoteCompositeVM.Text,
                    HardNoteId = id.ToString()
                };
            }
            if (compositeBaseVM is LineCompositeVM lineCompositeVM)
            {
                return new LineComposite()
                {
                    Id = lineCompositeVM.Id.ToString(),
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
                    Text = compositeBase.Text
                };
            }
            if (compositeBase.CompositeType == "HeaderComposite")
            {
                return new HeaderCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id),
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
                    Text = compositeBase.Quote
                };
            }
            if (compositeBase.CompositeType == "LineComposite")
            {
                return new LineCompositeVM()
                {
                    Id = Guid.Parse(compositeBase.Id)
                };
            }
            return null;
        }
    }
}