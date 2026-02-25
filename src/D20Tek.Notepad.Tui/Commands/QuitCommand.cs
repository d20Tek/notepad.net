namespace D20Tek.Notepad.Tui.Commands;

internal static class QuitCommand
{
    public const string CommandName = "Quit";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (UnsavedChangesHelper.PromptToSaveIfDirty(viewModel, "exiting") ==
            UnsavedChangesHelper.PromptResult.Cancel)
        {
            return;
        }

        Application.RequestStop();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel));
}
