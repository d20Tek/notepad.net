namespace D20Tek.Notepad.Tui.Menus;

internal sealed class MenuItemDefinition(string label, string commandName)
{
    public string Label { get; } = label;

    public string CommandName { get; } = commandName;
}
