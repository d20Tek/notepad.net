using D20Tek.Notepad.Tui.Commands;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuBuilder
{
    public static MenuBar Build(EditorViewModel viewModel)
    {
        var commands = new CommandRegistry();

        // File commands
        commands.Register(FileNewCommand.Create(viewModel));
        commands.Register(FileOpenCommand.Create(viewModel));
        commands.Register(FileSaveCommand.Create(viewModel));
        commands.Register(FileSaveAsCommand.Create(viewModel));
        commands.Register(QuitCommand.Create(viewModel));

        // Edit commands
        commands.Register(UndoCommand.Create(viewModel));
        commands.Register(RedoCommand.Create(viewModel));
        commands.Register(CutCommand.Create(viewModel));
        commands.Register(CopyCommand.Create(viewModel));
        commands.Register(PasteCommand.Create(viewModel));
        commands.Register(SelectAllCommand.Create(viewModel));

        // View commands
        commands.Register(WordWrapCommand.Create(viewModel));

        var menus = new[]
        {
            new MenuDefinition("_File",
                new MenuItemDefinition("_New", FileNewCommand.CommandName),
                MenuItemDefinition.Separator,
                new MenuItemDefinition("_Open...", FileOpenCommand.CommandName),
                new MenuItemDefinition("_Save", FileSaveCommand.CommandName),
                new MenuItemDefinition("Save _As...", FileSaveAsCommand.CommandName),
                MenuItemDefinition.Separator,
                new MenuItemDefinition("_Quit", QuitCommand.CommandName)),
            new MenuDefinition("_Edit",
                new MenuItemDefinition("_Undo", UndoCommand.CommandName, canExecute: () => viewModel.CanUndo),
                new MenuItemDefinition("_Redo", RedoCommand.CommandName, canExecute: () => viewModel.CanRedo),
                MenuItemDefinition.Separator,
                new MenuItemDefinition("Cu_t", CutCommand.CommandName, canExecute: () => viewModel.CanCut),
                new MenuItemDefinition("_Copy", CopyCommand.CommandName, canExecute: () => viewModel.CanCopy),
                new MenuItemDefinition("_Paste", PasteCommand.CommandName, canExecute: () => viewModel.CanPaste),
                MenuItemDefinition.Separator,
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

    public static MenuBar BuildMenu(IEnumerable<MenuDefinition> menus, CommandRegistry commands)
    {
        var menuBarItems = menus.Select(
            m => new MenuBarItem(m.Title, m.Items.Select(i => CreateMenuItem(i, commands)).ToArray())).ToArray();

        return new MenuBar(menuBarItems) { ColorScheme = EditorColorSchemes.MenuBar };
    }

    private static MenuItem? CreateMenuItem(MenuItemDefinition item, CommandRegistry commands)
    {
        // Separator returns null, which Terminal.Gui renders as a horizontal line
        if (item.IsSeparator) return null;

        var cmd = commands[item.CommandName];
        var shortcutKey = cmd.Shortcut ?? Key.Null;
        if (item.IsCheckable && item.IsChecked != null)
        {
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

        // Handle CanExecute for enabling/disabling menu items
        if (item.CanExecute != null)
        {
            return new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey)
            {
                CanExecute = item.CanExecute
            };
        }

        return new MenuItem(item.Label, "", () => cmd.Execute(), null, null, shortcutKey);
    }
}
