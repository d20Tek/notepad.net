namespace D20Tek.Notepad.Tui.Commands;

internal static class MoveLineUpCommand
{
    public const string CommandName = "MoveLineUp";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.MoveLineUp();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.AltMask | Key.CursorUp);
}

internal static class MoveLineDownCommand
{
    public const string CommandName = "MoveLineDown";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.MoveLineDown();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.AltMask | Key.CursorDown);
}
