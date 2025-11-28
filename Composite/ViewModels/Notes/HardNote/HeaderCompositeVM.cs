using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public partial class HeaderCompositeVM : CompositeBaseVM, IDisposable
    {
        [ObservableProperty] string _text = string.Empty;
        [ObservableProperty] string _fontWeight = string.Empty;
        [ObservableProperty] double _fontSize;

        public HeaderCompositeVM() => Id = Guid.NewGuid();

        public override object Clone() => new HeaderCompositeVM() { Id = Guid.NewGuid(), Tag = Tag, Comment = Comment, Text = Text, FontWeight = FontWeight, FontSize = FontSize };
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tag = string.Empty;
                Comment = string.Empty;
                Text = string.Empty;
                FontWeight = string.Empty;
                FontSize = 0;
            }
            base.Dispose(disposing);
        }
    }
}