namespace Composite.ViewModels.Notes.HardNote
{
    public class HeaderCompositeVM : CompositeBaseVM
    {
        public string Header { get; set; } = string.Empty;

        public HeaderCompositeVM()
        {
            Id = Guid.NewGuid();
        }
    }
}