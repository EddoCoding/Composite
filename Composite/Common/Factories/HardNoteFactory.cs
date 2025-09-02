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