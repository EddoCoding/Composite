namespace Composite.Services
{
    public class CommandService : ICommandService
    {
        Dictionary<string, Delegate> Commands = new Dictionary<string, Delegate>();

        public void RegsiterCommand(string keyboardShortcut, Delegate action)
        {
            if (!Commands.ContainsKey(keyboardShortcut)) Commands.Add(keyboardShortcut, action);
        }
        public void ExecuteCommand(string keyboardShortcut)
        {
            var command = SearchCommand(keyboardShortcut);
            command?.DynamicInvoke();
        }
        public void DeleteCommand(string keyboardShortcut) => Commands.Remove(keyboardShortcut);
        Delegate SearchCommand(string keyboardShortcut)
        {
            if (Commands.TryGetValue(keyboardShortcut, out Delegate commandAction)) return commandAction;
            else return null;
        }
    }
}