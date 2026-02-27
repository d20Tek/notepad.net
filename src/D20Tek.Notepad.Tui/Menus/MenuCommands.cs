using D20Tek.Notepad.Tui.Commands;

namespace D20Tek.Notepad.Tui.Menus;

internal static class MenuCommands
{
    public static CommandRegistry Get(EditorViewModel viewModel)
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

        // Search commands
        commands.Register(FindCommand.Create(viewModel));
        commands.Register(FindNextCommand.Create(viewModel));
        commands.Register(FindPreviousCommand.Create(viewModel));
        commands.Register(ReplaceCommand.Create(viewModel));
        commands.Register(GoToLineCommand.Create(viewModel));

        // View commands
        commands.Register(WordWrapCommand.Create(viewModel));

        return commands;
    }
}