using D20Tek.Notepad.Tui.Dialogs;

namespace D20Tek.Notepad.Tui.Commands;

internal static class GoToLineCommand
{
    public const string CommandName = "GoToLine";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        int currentLine = viewModel.Session.Caret.Line + 1;
        int totalLines = viewModel.Session.Document.LineCount;

        var dialog = GoToLineDialog.Create();
        dialog.Show(currentLine, totalLines);

        if (!dialog.Canceled && dialog.LineNumber.HasValue)
        {
            viewModel.GoToLine(dialog.LineNumber.Value);
        }
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.G);
}
