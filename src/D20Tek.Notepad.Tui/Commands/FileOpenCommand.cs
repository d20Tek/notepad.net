namespace D20Tek.Notepad.Tui.Commands;

internal static class FileOpenCommand
{
    public const string CommandName = "OpenFile";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (UnsavedChangesHelper.PromptToSaveIfDirty(viewModel, "opening a new file") ==
            UnsavedChangesHelper.PromptResult.Cancel)
        {
            return;
        }

        var dialog = new OpenDialog("Open File", "Select a file to open")
        {
            AllowsMultipleSelection = false
        };

        Application.Run(dialog);

        if (dialog.Canceled || dialog.FilePaths.Count == 0) return;

        FileLoadHelper.LoadAndApply(viewModel, dialog.FilePaths[0]);
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.O);
}
