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

    public static UiCommand Create(EditorViewModel viewModel, string filePath, int index) =>
        new($"{CommandPrefix}{index}", () => Execute(viewModel, filePath));
}
