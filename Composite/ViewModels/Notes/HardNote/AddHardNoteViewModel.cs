using Composite.ViewModels.Notes.HardNote;

namespace Composite.ViewModels.Notes.Note
{
    public class AddHardNoteViewModel : IDisposable
    {
        public HardNoteVM HardNoteVM { get; set; } = new();


        bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }
                _disposed = true;
            }
        }
    }
}