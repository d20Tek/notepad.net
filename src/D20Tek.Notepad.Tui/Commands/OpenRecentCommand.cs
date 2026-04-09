namespace D20Tek.Notepad.Tui.Commands;

internal static class OpenRecentCommand
{
    public const string CommandPrefix = "OpenRecent_";

    public static void Execute(EditorViewModel viewModel, string filePath)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (UnsavedChangesHelper.PromptToSaveIfDirty(viewModel, "opening a recent file") ==
            UnsavedChangesHelper.PromptResult.Cancel)
        {
            return;
        }

        FileLoadHelper.LoadAndApply(viewModel, filePath);
    }

    public static UiCommand Create(EditorViewModel viewModel, string filePath, int index) =>
        new($"{CommandPrefix}{index}", () => Execute(viewModel, filePath));
}
