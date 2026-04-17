namespace D20Tek.Notepad.Tui.Commands;

internal static class DuplicateLineCommand
{
    public const string CommandName = "DuplicateLine";

    public static void Execute(EditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        viewModel.DuplicateLine();
    }

    public static UiCommand Create(EditorViewModel viewModel) =>
        new(CommandName, () => Execute(viewModel), Key.CtrlMask | Key.ShiftMask | Key.D);
}
