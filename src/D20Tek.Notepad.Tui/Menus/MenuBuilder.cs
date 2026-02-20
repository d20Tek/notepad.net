using D20Tek.Notepad.Tui.Commands;
using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = new CommandRegistry();

        commands.Register(new FileOpenCommand("OpenFile", viewModel, Key.CtrlMask | Key.O));
        commands.Register(new UiCommand("Quit", () => Application.RequestStop()));

        var menus = new[]
        {
            new MenuDefinition("_File",
            new MenuItemDefinition("_Open...", "OpenFile"),
            new MenuItemDefinition("_Quit", "Quit"))
        };

        return BuildMenu(menus, commands);
    }

    public static MenuBar BuildMenu(IEnumerable<MenuDefinition> menus, CommandRegistry commands)
    {
        var menuBarItems = menus.Select(m =>
            new MenuBarItem(
                m.Title,
                m.Items.Select(i =>
                {
                    var cmd = commands[i.CommandName];
                    return new MenuItem(i.Label, "", () => cmd.Execute());
                }).ToArray()
            )).ToArray();

        return new MenuBar(menuBarItems)
        {
            ColorScheme = new ColorScheme
            {
                Normal = new Attribute(Color.White, Color.Black),
                Focus = new Attribute(Color.Black, Color.Gray),
                HotNormal = new Attribute(Color.BrightBlue, Color.DarkGray),
                HotFocus = new Attribute(Color.BrightBlue, Color.Gray)
            }
        };
    }
}
