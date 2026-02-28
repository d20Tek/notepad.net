using D20Tek.Notepad.Tui.Dialogs;

namespace D20Tek.Notepad.Tui.Commands;

internal static class FindCommand
{
    public const string CommandName = "Find";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        // Use selected text if available, otherwise fall back to last search term
        string initialSearchTerm = viewModel.Session.HasSelection
            ? viewModel.Session.GetSelectedText()
            : viewModel.LastSearchTerm ?? string.Empty;

        var dialog = FindDialog.Create();
        dialog.Show(initialSearchTerm, viewModel.SearchOptions);

        viewModel.SearchOptions = dialog.Options;

        switch (dialog.Action)
        {
            case FindDialog.FindAction.FindNext:
                if (!string.IsNullOrEmpty(dialog.SearchTerm))
                {
                    if (!viewModel.FindNext(dialog.SearchTerm))
                    {
                        MessageBox.Query("Find", "Text not found.", "OK");
                    }
                }
                break;

            case FindDialog.FindAction.FindPrevious:
                if (!string.IsNullOrEmpty(dialog.SearchTerm))
                {
                    if (!viewModel.FindPrevious(dialog.SearchTerm))
                    {
                        MessageBox.Query("Find", "Text not found.", "OK");
                    }
                }
                break;
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.F);
}

internal static class FindNextCommand
{
    public const string CommandName = "FindNext";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (!viewModel.HasLastSearch)
        {
            MessageBox.Query("Find Next", "No previous search. Use Find (Ctrl+F) first.", "OK");
            return;
        }

        if (!viewModel.FindNextFromCurrent())
        {
            MessageBox.Query("Find Next", "Text not found.", "OK");
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.F3);
}

internal static class FindPreviousCommand
{
    public const string CommandName = "FindPrevious";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        if (!viewModel.HasLastSearch)
        {
            MessageBox.Query("Find Previous", "No previous search. Use Find (Ctrl+F) first.", "OK");
            return;
        }

        if (!viewModel.FindPreviousFromCurrent())
        {
            MessageBox.Query("Find Previous", "Text not found.", "OK");
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.ShiftMask | Key.F3);
}
