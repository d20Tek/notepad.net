namespace D20Tek.Notepad.Tui.Commands;

internal static class FileSaveAsCommand
{
    public const string CommandName = "SaveAsFile";

    public static bool Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.EndTypingGroupIfNeeded();

        var dialog = new SaveDialog("Save As", "Select a file to save")
        {
            AllowsOtherFileTypes = true
        };

        Application.Run(dialog);

        if (dialog.Canceled || string.IsNullOrEmpty(dialog.FilePath?.ToString()))
        {
            return false;
        }

        string filePath = dialog.FilePath.ToString()!;
        return FileSaveCommand.SaveToFile(viewModel, filePath);
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.ShiftMask | Key.S);
}
