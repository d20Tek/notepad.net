namespace D20Tek.Notepad.Tui.Menus;

internal abstract record MenuEntry
{
    public static MenuEntry Separator { get; } = new SeparatorEntry();
}

internal sealed record SeparatorEntry() : MenuEntry;

internal sealed record CommandEntry(
    string Label,
    string CommandName,
    bool IsCheckable = false,
    Func<bool>? IsChecked = null,
    Func<bool>? CanExecute = null) : MenuEntry;

internal sealed record SubMenuEntry(string Label, IReadOnlyList<MenuEntry> Items) : MenuEntry
{
    public SubMenuEntry(string label, params MenuEntry[] items)
        : this(label, (IReadOnlyList<MenuEntry>)items) { }
}

internal sealed record ActionEntry(string Label, Action Execute, Func<bool>? CanExecute = null) : MenuEntry;
