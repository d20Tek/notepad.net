namespace D20Tek.Notepad.Tui.Commands;

internal sealed class CommandRegistry
{
    private readonly Dictionary<string, UiCommand> _commands = [];

    public void Register(UiCommand command) => _commands[command.Name] = command;

    public UiCommand this[string name] => _commands[name];
}
