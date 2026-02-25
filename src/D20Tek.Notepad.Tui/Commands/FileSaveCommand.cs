namespace D20Tek.Notepad.Tui.Commands;

internal static class FileSaveCommand
{
    public const string CommandName = "SaveFile";

    public static bool Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.EndTypingGroupIfNeeded();

        // If no file path, fall back to Save As
        if (string.IsNullOrEmpty(viewModel.CurrentFilePath))
        {
            return FileSaveAsCommand.Execute(viewModel);
        }

        return SaveToFile(viewModel, viewModel.CurrentFilePath);
    }

    internal static bool SaveToFile(EditorViewModel viewModel, string filePath)
    {
        try
        {
            var factory = new DocumentFactory();
            factory.Save(viewModel.Session.Document, filePath);

            viewModel.SetFilePath(filePath);
            viewModel.ClearDirtyFlag();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to save file:\n{ex.Message}", "OK");
            return false;
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.S);
}
