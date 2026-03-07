using D20Tek.Notepad.Tui.Commands;
using System.Diagnostics;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = MenuCommands.Get(viewModel);
        var menuBarItems = MenuDefinitions.Get(viewModel)
            .Select(m => new MenuBarItem(m.Title, BuildItems(m.Items, commands)))
            .ToArray();

        return new MenuBar(menuBarItems) { ColorScheme = EditorColorSchemes.MenuBar };
    }

    private static MenuItem?[] BuildItems(IReadOnlyList<MenuEntry> entries, CommandRegistry commands) =>
        entries.Select(e => BuildItem(e, commands)).ToArray();

    private static MenuItem? BuildItem(MenuEntry entry, CommandRegistry commands) => entry switch
    {
        SeparatorEntry => null,
        SubMenuEntry sub => new MenuBarItem(sub.Label, BuildItems(sub.Items, commands)),
        CommandEntry cmd => BuildCommandItem(cmd, commands),
        _ => throw new UnreachableException($"Unhandled MenuEntry: {entry.GetType().Name}")
    };

    private static MenuItem BuildCommandItem(CommandEntry item, CommandRegistry commands)
    {
        var cmd = commands[item.CommandName];
        var shortcutKey = cmd.Shortcut ?? Key.Null;
        return item switch
        {
            { IsCheckable: true, IsChecked: not null } => BuildCheckableItem(item, cmd, shortcutKey),
            { CanExecute: not null } =>
                new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey)
                {
                    CanExecute = item.CanExecute
                },
            _ => new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey)
        };
    }

    private static MenuItem BuildCheckableItem(CommandEntry item, UiCommand cmd, Key shortcutKey)
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

