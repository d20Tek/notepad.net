using D20Tek.Notepad.Tui.Dialogs;

namespace D20Tek.Notepad.Tui.Commands;

internal static class ReplaceCommand
{
    public const string CommandName = "Replace";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        var dialog = ReplaceDialog.Create();
        dialog.Show(viewModel.LastSearchTerm ?? string.Empty, string.Empty, viewModel.SearchOptions);

        viewModel.SearchOptions = dialog.Options;

        switch (dialog.Action)
        {
            case ReplaceDialog.ReplaceAction.FindNext:
                if (!string.IsNullOrEmpty(dialog.SearchTerm))
                {
                    if (!viewModel.FindNext(dialog.SearchTerm))
                    {
                        MessageBox.Query("Find", "Text not found.", "OK");
                    }
                }
                break;

            case ReplaceDialog.ReplaceAction.Replace:
                if (!string.IsNullOrEmpty(dialog.SearchTerm))
                {
                    if (!viewModel.Replace(dialog.SearchTerm, dialog.ReplacementText))
                    {
                        MessageBox.Query("Replace", "Text not found.", "OK");
                    }
                }
                break;

            case ReplaceDialog.ReplaceAction.ReplaceAll:
                if (!string.IsNullOrEmpty(dialog.SearchTerm))
                {
                    int count = viewModel.ReplaceAll(dialog.SearchTerm, dialog.ReplacementText);
                    MessageBox.Query("Replace All", $"Replaced {count} occurrence(s).", "OK");
                }
                break;
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.H);
}
