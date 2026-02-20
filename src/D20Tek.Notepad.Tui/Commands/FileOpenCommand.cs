namespace D20Tek.Notepad.Tui.Commands;

internal sealed class FileOpenCommand(string name, EditorViewModel viewModel, Key? shortcut = null)
    : UiCommand(name, () => Handle(viewModel), shortcut)
{
    private static void Handle(EditorViewModel viewModel)
    {
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
            viewModel.Viewport.ResetViewport();
            viewModel.Refresh();
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open file:\n{ex.Message}", "OK");
        }
    }
}
