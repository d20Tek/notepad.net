using D20Tek.Notepad.Tui.Commands;
using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = new CommandRegistry();

        commands.Register(new UiCommand("OpenFile", () => FileOpenCommand.Execute(viewModel), Key.CtrlMask | Key.O));
        commands.Register(WordWrapCommand.Create(viewModel));
        commands.Register(new UiCommand("Quit", () => Application.RequestStop()));

        var menus = new[]
        {
            new MenuDefinition("_File",
                new MenuItemDefinition("_Open...", "OpenFile"),
                new MenuItemDefinition("_Quit", "Quit")),
            new MenuDefinition("_View",
                new MenuItemDefinition(
                    "_Word Wrap",
                    WordWrapCommand.CommandName,
                    isCheckable: true,
                    isChecked: () => viewModel.IsWordWrapEnabled))
        };

        return BuildMenu(menus, commands);
    }

    public static MenuBar BuildMenu(IEnumerable<MenuDefinition> menus, CommandRegistry commands)
    {
        var menuBarItems = menus.Select(
            m =>new MenuBarItem(m.Title, m.Items.Select(i => CreateMenuItem(i, commands)).ToArray()))
            .ToArray();

        return new MenuBar(menuBarItems)
        {
            ColorScheme = new ColorScheme
            {
                Normal = new Attribute(Color.White, Color.Black),
                Focus = new Attribute(Color.Black, Color.Gray),
                HotNormal = new Attribute(Color.BrightBlue, Color.Black),
                HotFocus = new Attribute(Color.BrightBlue, Color.DarkGray)
            }
        };
    }

    private static MenuItem CreateMenuItem(MenuItemDefinition item, CommandRegistry commands)
    {
        var cmd = commands[item.CommandName];

        if (item.IsCheckable && item.IsChecked != null)
        {
            var menuItem = new MenuItem(item.Label, "", () => cmd.Execute())
            {
                CheckType = MenuItemCheckStyle.Checked
            };

            // Update checked state before display
            menuItem.Action = () =>
            {
                cmd.Execute();
                menuItem.Checked = item.IsChecked();
            };

            // Set initial checked state
            menuItem.Checked = item.IsChecked();

            return menuItem;
        }

        return new MenuItem(item.Label, "", () => cmd.Execute());
    }
}
