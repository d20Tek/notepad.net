using System.Diagnostics;

namespace D20Tek.Notepad.Tui.Commands;

internal static class HelpGettingStartedCommand
{
    public const string CommandName = "HelpGettingStarted";
    private const string Url = "https://d20tek.com/projects/notepad-tui";

    public static void Execute()
    {
        try
        {
            Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Could not open browser:\n{ex.Message}", "OK");
        }
    }

    public static UiCommand Create() => new(CommandName, Execute, Key.F1);
}
