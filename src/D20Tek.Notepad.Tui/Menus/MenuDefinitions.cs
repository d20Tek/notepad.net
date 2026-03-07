using D20Tek.Notepad.Tui.Commands;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuDefinitions
{
    public static TopLevelMenu[] Get(EditorViewModel viewModel) =>
    [
        new TopLevelMenu("_File",
            new CommandEntry("_New", FileNewCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Open...", FileOpenCommand.CommandName),
            new CommandEntry("_Save", FileSaveCommand.CommandName),
            new CommandEntry("Save _As...", FileSaveAsCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Quit", QuitCommand.CommandName)),

        new TopLevelMenu("_Edit",
            new CommandEntry("_Undo", UndoCommand.CommandName, CanExecute: () => viewModel.CanUndo),
            new CommandEntry("_Redo", RedoCommand.CommandName, CanExecute: () => viewModel.CanRedo),
            MenuEntry.Separator,
            new CommandEntry("Cu_t", CutCommand.CommandName, CanExecute: () => viewModel.CanCut),
            new CommandEntry("_Copy", CopyCommand.CommandName, CanExecute: () => viewModel.CanCopy),
            new CommandEntry("_Paste", PasteCommand.CommandName, CanExecute: () => viewModel.CanPaste),
            MenuEntry.Separator,
            new CommandEntry("_Find...", FindCommand.CommandName),
            new CommandEntry("Find _Next", FindNextCommand.CommandName, CanExecute: () => viewModel.HasLastSearch),
            new CommandEntry("Find Pre_vious", FindPreviousCommand.CommandName, CanExecute: () => viewModel.HasLastSearch),
            new CommandEntry("_Replace...", ReplaceCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Go to Line...", GoToLineCommand.CommandName),
            new CommandEntry("Select _All", SelectAllCommand.CommandName)),

        new TopLevelMenu("_View",
            new CommandEntry(
                "_Status Bar",
                StatusBarCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsStatusBarEnabled),
            new CommandEntry(
                "_Word Wrap",
                WordWrapCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsWordWrapEnabled))
    ];
}

