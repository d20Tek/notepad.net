namespace D20Tek.Notepad.Tui.Commands;

internal static class OverwriteModeCommand
{
    public const string CommandName = "ToggleOverwriteMode";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.ToggleInsertMode();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.InsertChar);
}
