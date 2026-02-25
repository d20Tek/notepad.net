namespace D20Tek.Notepad.Tui.Menus;

internal sealed class MenuItemDefinition
{
    public static readonly MenuItemDefinition Separator = new(isSeparator: true);

    public string Label { get; }
    public string CommandName { get; }
    public bool IsCheckable { get; }
    public Func<bool>? IsChecked { get; }
    public Func<bool>? CanExecute { get; }
    public bool IsSeparator { get; }

    // Standard menu item constructor
    public MenuItemDefinition(
        string label,
        string commandName,
        bool isCheckable = false,
        Func<bool>? isChecked = null,
        Func<bool>? canExecute = null)
    {
        Label = label;
        CommandName = commandName;
        IsCheckable = isCheckable;
        IsChecked = isChecked;
        CanExecute = canExecute;
        IsSeparator = false;
    }

    // Private constructor for separator
    private MenuItemDefinition(bool isSeparator)
    {
        Label = string.Empty;
        CommandName = string.Empty;
        IsSeparator = isSeparator;
    }
}
