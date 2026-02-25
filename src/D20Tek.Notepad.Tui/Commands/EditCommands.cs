namespace D20Tek.Notepad.Tui.Commands;

internal static class UndoCommand
{
    public const string CommandName = "Undo";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.Undo();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.Z);
}

internal static class RedoCommand
{
    public const string CommandName = "Redo";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.Redo();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.Y);
}

internal static class SelectAllCommand
{
    public const string CommandName = "SelectAll";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.SelectAll();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.A);
}
