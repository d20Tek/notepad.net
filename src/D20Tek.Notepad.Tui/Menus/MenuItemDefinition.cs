namespace D20Tek.Notepad.Tui.Menus;

internal sealed class MenuItemDefinition(string label, string commandName, bool isCheckable = false, Func<bool>? isChecked = null)
{
    public string Label { get; } = label;

    public string CommandName { get; } = commandName;

    public bool IsCheckable { get; } = isCheckable;

    public Func<bool>? IsChecked { get; } = isChecked;
}
