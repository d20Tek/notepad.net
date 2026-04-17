namespace D20Tek.Notepad.Tui.Commands;

internal static class TrimTrailingWhitespaceCommand
{
    public const string CommandName = "TrimTrailingWhitespace";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.TrimTrailingWhitespace();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}
