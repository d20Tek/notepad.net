namespace D20Tek.Notepad.Tui.Commands;

internal static class UnsavedChangesHelper
{
    public enum PromptResult
    {
        Proceed,
        Cancel
    }

    public static PromptResult PromptToSaveIfDirty(EditorViewModel viewModel, string actionDescription)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.EndTypingGroupIfNeeded();

        if (!viewModel.IsDirty)
        {
            return PromptResult.Proceed;
        }

        int result = MessageBox.Query(
            "Unsaved Changes",
            $"Do you want to save changes before {actionDescription}?",
            "Save",
            "Don't Save",
            "Cancel");

        return result switch
        {
            0 => FileSaveCommand.Execute(viewModel) ? PromptResult.Proceed : PromptResult.Cancel,
            1 => PromptResult.Proceed,
            _ => PromptResult.Cancel
        };
    }
}
