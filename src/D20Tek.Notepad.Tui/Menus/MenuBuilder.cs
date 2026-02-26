using D20Tek.Notepad.Tui.Commands;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = MenuCommands.Get(viewModel);
        var menuBarItems = MenuDefinitions.Get(viewModel).Select(
            m => new MenuBarItem(m.Title, m.Items.Select(i => CreateMenuItem(i, commands)).ToArray())).ToArray();

        return new MenuBar(menuBarItems) { ColorScheme = EditorColorSchemes.MenuBar };
    }

    private static MenuItem? CreateMenuItem(MenuItemDefinition item, CommandRegistry commands)
    {
        if (item.IsSeparator) return null;      // Terminal.Gui uses null to indicate a separator

        var cmd = commands[item.CommandName];
        var shortcutKey = cmd.Shortcut ?? Key.Null;
        return item switch
        {
            { IsCheckable: true, IsChecked: not null } => CreateCheckableMenuItem(item, cmd, shortcutKey),
            { CanExecute: not null } =>
                new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey)
                {
                    CanExecute = item.CanExecute
                },
            _ => new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey)
        };
    }

    private static MenuItem CreateCheckableMenuItem(MenuItemDefinition item, UiCommand cmd, Key shortcutKey)
    {
        ArgumentNullException.ThrowIfNull(item.IsChecked);

        var menuItem = new MenuItem(item.Label, "", () => cmd.Execute())
        {
            Shortcut = shortcutKey,
            CheckType = MenuItemCheckStyle.Checked
        };

        menuItem.Action = () =>
        {
            cmd.Execute();
            menuItem.Checked = item.IsChecked();
        };

        menuItem.Checked = item.IsChecked();
        return menuItem;
    }
}
