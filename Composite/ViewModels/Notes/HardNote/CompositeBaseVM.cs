using CommunityToolkit.Mvvm.ComponentModel;

namespace Composite.ViewModels.Notes.HardNote
{
    public abstract partial class CompositeBaseVM : ObservableObject, ICloneable, IDisposable
    {
        public Guid Id { get; set; }
        [ObservableProperty] string _tag;
        [ObservableProperty] string _comment;
        [ObservableProperty] bool _isEditing;
        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        public abstract object Clone();

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
        }
        ~CompositeBaseVM() => Dispose(false);
    }
}