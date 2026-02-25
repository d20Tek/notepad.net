namespace D20Tek.Notepad.Tui.Commands;

internal static class FileOpenCommand
{
    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        // Check for unsaved changes
        if (viewModel.IsDirty)
        {
            int result = MessageBox.Query(
                "Unsaved Changes",
                "Do you want to save changes before opening a new file?",
                "Save",
                "Don't Save",
                "Cancel");

            switch (result)
            {
                case 0: // Save
                    if (!FileSaveCommand.Execute(viewModel))
                    {
                        return; // Save was cancelled
                    }
                    break;
                case 1: // Don't Save
                    break;
                case 2: // Cancel
                case -1: // Escape
                    return;
            }
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
            viewModel.SetFilePath(Path.GetFullPath(filePath));
            viewModel.ClearDirtyFlag();
            viewModel.Viewport.Reset();
            viewModel.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open file:\n{ex.Message}", "OK");
        }
    }
}
