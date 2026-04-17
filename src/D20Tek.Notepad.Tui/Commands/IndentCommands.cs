namespace D20Tek.Notepad.Tui.Commands;

internal static class IndentSelectionCommand
{
    public const string CommandName = "IndentSelection";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.IndentSelection();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}

internal static class OutdentSelectionCommand
{
    public const string CommandName = "OutdentSelection";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.OutdentSelection();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}
