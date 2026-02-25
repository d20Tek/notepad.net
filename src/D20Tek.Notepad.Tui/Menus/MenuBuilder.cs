using D20Tek.Notepad.Tui.Commands;
using Attribute = Terminal.Gui.Attribute;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = new CommandRegistry();

        // File commands
        commands.Register(FileNewCommand.Create(viewModel));
        commands.Register(new UiCommand("OpenFile", () => FileOpenCommand.Execute(viewModel), Key.CtrlMask | Key.O));
        commands.Register(FileSaveCommand.Create(viewModel));
        commands.Register(FileSaveAsCommand.Create(viewModel));
        commands.Register(new UiCommand("Quit", () => RequestQuit(viewModel)));

        // Edit commands
        commands.Register(UndoCommand.Create(viewModel));
        commands.Register(RedoCommand.Create(viewModel));
        commands.Register(SelectAllCommand.Create(viewModel));

        // View commands
        commands.Register(WordWrapCommand.Create(viewModel));

        var menus = new[]
        {
            new MenuDefinition("_File",
                new MenuItemDefinition("_New", FileNewCommand.CommandName),
                new MenuItemDefinition("_Open...", "OpenFile"),
                new MenuItemDefinition("_Save", FileSaveCommand.CommandName),
                new MenuItemDefinition("Save _As...", FileSaveAsCommand.CommandName),
                new MenuItemDefinition("_Quit", "Quit")),
            new MenuDefinition("_Edit",
                new MenuItemDefinition("_Undo", UndoCommand.CommandName),
                new MenuItemDefinition("_Redo", RedoCommand.CommandName),
                new MenuItemDefinition("Select _All", SelectAllCommand.CommandName)),
            new MenuDefinition("_View",
                new MenuItemDefinition(
                    "_Word Wrap",
                    WordWrapCommand.CommandName,
                    isCheckable: true,
                    isChecked: () => viewModel.IsWordWrapEnabled))
        };

        return BuildMenu(menus, commands);
    }

    private static void RequestQuit(EditorViewModel viewModel)
    {
        if (viewModel.IsDirty)
        {
            int result = MessageBox.Query(
                "Unsaved Changes",
                "Do you want to save changes before exiting?",
                "Save",
                "Don't Save",
                "Cancel");

            switch (result)
            {
                case 0: // Save
                    if (!FileSaveCommand.Execute(viewModel))
                    {
                        return; // Save was cancelled
                    }
                    break;
                case 1: // Don't Save
                    break;
                case 2: // Cancel
                case -1: // Escape
                    return;
            }
        }

        Application.RequestStop();
    }

    public static MenuBar BuildMenu(IEnumerable<MenuDefinition> menus, CommandRegistry commands)
    {
        var menuBarItems = menus.Select(
            m => new MenuBarItem(m.Title, m.Items.Select(i => CreateMenuItem(i, commands)).ToArray()))
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
