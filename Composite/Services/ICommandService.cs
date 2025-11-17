namespace Composite.Services
{
    public interface ICommandService
    {
        void RegsiterCommand(string keyboardShortcut, Delegate action);
        void ExecuteCommand(string keyboardShortcut);
        void DeleteCommand(string keyboardShortcut);
    }
}