namespace D20Tek.Notepad.Tui.Menus;

internal sealed record TopLevelMenu(string Title, IReadOnlyList<MenuEntry> Items)
{
    public TopLevelMenu(string title, params MenuEntry[] items)
        : this(title, (IReadOnlyList<MenuEntry>)items) { }
}
