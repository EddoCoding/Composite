using System.Windows.Input;

namespace Composite.ViewModels.Notes.HardNote
{
    public class CommonCommandMenu
    {
        public string NameCommand { get; set; } = string.Empty;
        public string PathImage { get; set; } = string.Empty;
        public ICommand ActionCommand { get; set; }

        public CommonCommandMenu(string name, string pathImage, ICommand command)
        {
            NameCommand = name;
            PathImage = pathImage;
            ActionCommand = command;
        }
    }
}