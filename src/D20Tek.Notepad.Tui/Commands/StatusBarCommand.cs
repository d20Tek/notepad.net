namespace D20Tek.Notepad.Tui.Commands;

internal static class StatusBarCommand
{
    public const string CommandName = "ToggleStatusBar";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.ToggleStatusBar();
    }

    public static UiCommand Create(EditorViewModel viewModel) => new(CommandName, () => Execute(viewModel));
}
