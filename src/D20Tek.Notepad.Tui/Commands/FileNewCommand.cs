namespace D20Tek.Notepad.Tui.Commands;

internal static class FileNewCommand
{
    public const string CommandName = "NewFile";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        // Check for unsaved changes
        if (viewModel.IsDirty)
        {
            int result = MessageBox.Query(
                "Unsaved Changes",
                "Do you want to save changes before creating a new file?",
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

        // Create new empty document
        var factory = new DocumentFactory();
        var newDoc = DocumentFactory.CreateEmpty();

        viewModel.Session.ReplaceDocument(newDoc);
        viewModel.ClearFilePath();
        viewModel.ClearDirtyFlag();
        viewModel.Viewport.Reset();
        viewModel.Refresh();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.N);
}
