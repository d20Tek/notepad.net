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

        string filePath = dialog.FilePaths[0];

        try
        {
            var factory = new DocumentFactory();
            var newDoc = factory.Load(filePath);

            viewModel.Session.ReplaceDocument(newDoc);
            viewModel.Session.UndoStack.Clear(); 
            viewModel.SetFilePath(Path.GetFullPath(filePath));
            viewModel.ResetCleanVersion();
            viewModel.Viewport.Reset();
            viewModel.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open file:\n{ex.Message}", "OK");
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.O);
}
