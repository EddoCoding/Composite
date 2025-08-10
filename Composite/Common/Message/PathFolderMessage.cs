namespace Composite.Common.Message
{
    public class PathFolderMessage(string path)
    {
        public string Path { get; set; } = path;
    }
}