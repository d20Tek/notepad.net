namespace D20Tek.Notepad.Tui.Commands;

internal class UiCommand(string name, Action execute, Key? shortcut = null)
{
    public string Name { get; } = name;

    public Action Execute { get; } = execute;

    public Key? Shortcut { get; } = shortcut;
}
