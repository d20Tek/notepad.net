using D20Tek.Notepad.Tui.Dialogs;

namespace D20Tek.Notepad.Tui.Commands;

internal static class HelpAboutCommand
{
    public const string CommandName = "HelpAbout";

    public static void Execute() => AboutDialog.Create().Show();

    public static UiCommand Create() => new(CommandName, Execute);
}
