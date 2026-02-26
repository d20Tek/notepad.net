using D20Tek.Notepad.Tui.Commands;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuDefinitions
{
    public static MenuDefinition[] Get(EditorViewModel viewModel) =>
    [
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
    ];
}
