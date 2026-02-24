namespace D20Tek.Notepad.Tui.Commands;

internal static class WordWrapCommand
{
    public const string CommandName = "ToggleWordWrap";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.ToggleWordWrap();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}
