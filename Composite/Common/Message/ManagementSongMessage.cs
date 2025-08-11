using Composite.ViewModels;

namespace Composite.Common.Message
{
    public class ManagementSongMessage(string message, Guid? id = null, SongVM songVM = null)
    {
        public Guid? Id { get; set; } = id;
        public string Message { get; set; } = message;
        public SongVM SongVM { get; set; } = songVM;
    }
}