using System.Windows.Input;

namespace Composite.ViewModels.Notes.HardNote
{
    public class CommandContextMenu
    {
        public string NameCommand { get; set; }
        public ICommand ActionCommand { get; set; }

        public CommandContextMenu(string name, ICommand command)
        {
            NameCommand = name;
            ActionCommand = command;
        }
    }
}