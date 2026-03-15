namespace D20Tek.Notepad.Tui.Commands;

internal static class ZoomCommands
{
    public const string ZoomInCommandName = "ZoomIn";
    public const string ZoomOutCommandName = "ZoomOut";

    private const string HintMessage = "Zoom: use Ctrl+Scroll or Ctrl+Plus/Minus in your terminal (e.g. Windows Terminal)";
    private const string MessageBoxTitle = "Zoom";
    private const string MessageBoxText =
        "Text zoom is controlled by your terminal emulator.\n\n" +
        "Use Ctrl+Scroll or Ctrl+Plus/Minus\nin Windows Terminal or your terminal app.";

    public static void ShowHint(EditorViewModel viewModel, StatusBarView statusBarView)
    {
        if (viewModel.IsStatusBarEnabled)
            statusBarView.ShowTransientMessage(HintMessage, TimeSpan.FromSeconds(4));
        else
            MessageBox.Query(MessageBoxTitle, MessageBoxText, "OK");
    }

    public static UiCommand CreateZoomIn(EditorViewModel viewModel, StatusBarView statusBarView) =>
        new(ZoomInCommandName, () => ShowHint(viewModel, statusBarView), Key.CtrlMask | (Key)'=');

    public static UiCommand CreateZoomOut(EditorViewModel viewModel, StatusBarView statusBarView) =>
        new(ZoomOutCommandName, () => ShowHint(viewModel, statusBarView), Key.CtrlMask | (Key)'-');
}
