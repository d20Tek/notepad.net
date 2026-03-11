namespace D20Tek.Notepad.Tui.Commands;

internal static class LineNumbersCommand
{
    public const string CommandName = "ToggleLineNumbers";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.ToggleLineNumbers();
    }

    public static UiCommand Create(EditorViewModel viewModel) => new(CommandName, () => Execute(viewModel));
}
