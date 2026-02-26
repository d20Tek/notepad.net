namespace D20Tek.Notepad.Tui.Commands;

internal static class QuitCommand
{
    public const string CommandName = "Quit";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        UnsavedChangesHelper.PromptToSaveIfDirty(viewModel, "exiting");
        Application.RequestStop();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}
