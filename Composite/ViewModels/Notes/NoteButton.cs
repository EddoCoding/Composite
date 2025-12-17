namespace Composite.ViewModels.Notes
{
    public partial class NoteButton : NoteBaseVM
    {
        public override string ItemType => "AddNoteButton";
        public NoteButton() => Title = "+ Composite";
    }
}