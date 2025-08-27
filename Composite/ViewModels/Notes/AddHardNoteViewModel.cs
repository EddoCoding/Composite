namespace Composite.ViewModels.Notes
{
    public class AddHardNoteViewModel : IDisposable
    {


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