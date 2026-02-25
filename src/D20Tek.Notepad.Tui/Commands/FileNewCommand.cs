namespace D20Tek.Notepad.Tui.Commands;

internal static class FileNewCommand
{
    public const string CommandName = "NewFile";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (UnsavedChangesHelper.PromptToSaveIfDirty(viewModel, "creating a new file") ==
            UnsavedChangesHelper.PromptResult.Cancel)
        {
            return;
        }

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
