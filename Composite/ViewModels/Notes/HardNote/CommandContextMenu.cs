using System.Windows.Input;

namespace Composite.ViewModels.Notes.HardNote
{
    public class CommandContextMenu : IDisposable
    {
        public string NameCommand { get; set; } = string.Empty;
        public string PathImage { get; set; } = string.Empty;
        public ICommand ActionCommand { get; set; }

        public CommandContextMenu(string name, string pathImage, ICommand command)
        {
            NameCommand = name;
            PathImage = pathImage;
            ActionCommand = command;
        }

        public void Dispose()
        {
            NameCommand = string.Empty;
            PathImage = string.Empty;
            ActionCommand = null;
        }
    }
}