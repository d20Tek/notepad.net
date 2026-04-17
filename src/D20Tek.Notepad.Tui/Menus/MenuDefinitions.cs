using D20Tek.Notepad.Tui.Commands;
using System.Text;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuDefinitions
{
    public static TopLevelMenu[] Get(EditorViewModel viewModel) =>
    [
        new TopLevelMenu("_File",
            new CommandEntry("_New", FileNewCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Open...", FileOpenCommand.CommandName),
            new SubMenuEntry("_Recent", BuildRecentFileItems(viewModel)),
            new CommandEntry("_Save", FileSaveCommand.CommandName),
            new CommandEntry("Save _As...", FileSaveAsCommand.CommandName),
            MenuEntry.Separator,
            new SubMenuEntry("Enco_ding",
                new CommandEntry(
                    "UTF-_8",
                    ChangeEncodingCommand.Utf8CommandName,
                    IsCheckable: true,
                    IsChecked: () => viewModel.IsCurrentEncoding(new UTF8Encoding(false))),
                new CommandEntry(
                    "UTF-8 with _BOM",
                    ChangeEncodingCommand.Utf8BomCommandName,
                    IsCheckable: true,
                    IsChecked: () => viewModel.IsCurrentEncoding(new UTF8Encoding(true))),
                new CommandEntry(
                    "_Unicode (UTF-16 LE)",
                    ChangeEncodingCommand.UnicodeCommandName,
                    IsCheckable: true,
                    IsChecked: () => viewModel.IsCurrentEncoding(Encoding.Unicode)),
                new CommandEntry(
                    "_BigEndian Unicode (UTF-16 BE)",
                    ChangeEncodingCommand.BigEndianUnicodeCommandName,
                    IsCheckable: true,
                    IsChecked: () => viewModel.IsCurrentEncoding(Encoding.BigEndianUnicode))),
            MenuEntry.Separator,
            new CommandEntry("_Quit", QuitCommand.CommandName)),

        new TopLevelMenu("_Edit",
            new CommandEntry("_Undo", UndoCommand.CommandName, CanExecute: () => viewModel.CanUndo),
            new CommandEntry("_Redo", RedoCommand.CommandName, CanExecute: () => viewModel.CanRedo),
            MenuEntry.Separator,
            new CommandEntry("Cu_t", CutCommand.CommandName, CanExecute: () => viewModel.CanCut),
            new CommandEntry("_Copy", CopyCommand.CommandName, CanExecute: () => viewModel.CanCopy),
            new CommandEntry("_Paste", PasteCommand.CommandName, CanExecute: () => viewModel.CanPaste),
            new CommandEntry("Select _All", SelectAllCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Duplicate Line", DuplicateLineCommand.CommandName),
            new CommandEntry("Move Line _Up", MoveLineUpCommand.CommandName),
            new CommandEntry("Move Line Do_wn", MoveLineDownCommand.CommandName),
            new CommandEntry("_Indent Selection", IndentSelectionCommand.CommandName),
            new CommandEntry("Outden_t Selection", OutdentSelectionCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("Tri_m Trailing Whitespace", TrimTrailingWhitespaceCommand.CommandName)),

        new TopLevelMenu("Searc_h",
            new CommandEntry("_Find...", FindCommand.CommandName),
            new CommandEntry("Find _Next", FindNextCommand.CommandName, CanExecute: () => viewModel.HasLastSearch),
            new CommandEntry("Find Prev_ious", FindPreviousCommand.CommandName, CanExecute: () => viewModel.HasLastSearch),
            new CommandEntry("_Replace...", ReplaceCommand.CommandName),
            MenuEntry.Separator,
            new CommandEntry("_Go to Line...", GoToLineCommand.CommandName)),

        new TopLevelMenu("_View",
            new CommandEntry("Zoom _In", ZoomCommands.ZoomInCommandName),
            new CommandEntry("Zoom _Out", ZoomCommands.ZoomOutCommandName),
            MenuEntry.Separator,
            new CommandEntry(
                "_Status Bar",
                StatusBarCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsStatusBarEnabled),
            new CommandEntry(
                "_Word Wrap",
                WordWrapCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsWordWrapEnabled),
            new CommandEntry(
                "_Line Numbers",
                LineNumbersCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsLineNumbersEnabled),
            MenuEntry.Separator,
            new CommandEntry(
                "_Overwrite Mode",
                OverwriteModeCommand.CommandName,
                IsCheckable: true,
                IsChecked: () => viewModel.IsOverwriteMode)),

        new TopLevelMenu("_Help",
            new CommandEntry("_Getting Started", HelpGettingStartedCommand.CommandName),
            new CommandEntry("_About Notepad.Tui", HelpAboutCommand.CommandName))
    ];

    private static IReadOnlyList<MenuEntry> BuildRecentFileItems(EditorViewModel viewModel)
    {
        if (viewModel.RecentFiles.Count == 0) return [new ActionEntry("(Empty)", () => { }, CanExecute: () => false)];

        return [.. viewModel.RecentFiles
                            .Select((path, i) =>
                            {
                                var label = $"_{(i == 9 ? 0 : i + 1)} {Path.GetFileName(path)}";
                                return (MenuEntry)new ActionEntry(
                                    label,
                                    () => OpenRecentCommand.Execute(viewModel, path));
                            })];
    }
}
