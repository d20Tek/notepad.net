namespace D20Tek.Notepad.Tui.Menus;

internal sealed class MenuDefinition(string title, params MenuItemDefinition[] items)
{
    public string Title { get; } = title;

    public IReadOnlyList<MenuItemDefinition> Items { get; } = items;
}
