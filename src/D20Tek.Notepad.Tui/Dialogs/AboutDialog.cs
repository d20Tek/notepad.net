using System.Reflection;

namespace D20Tek.Notepad.Tui.Dialogs;

internal sealed class AboutDialog
{
    private const string AppName = "Notepad.Tui";
    private const string Description = "A fast, terminal-based text editor for .NET,\ninspired by classic Notepad with no frills.";
    private const string Copyright = "Copyright (c) d20Tek.  All rights reserved.";
    private const string Repository = "https://github.com/d20Tek/notepad.net";

    public void Show()
    {
        var version = GetVersion();
        var dialog = new Dialog("About Notepad.Tui", 60, 14);

        var nameLabel = new Label(AppName) { X = 2, Y = 1 };
        var versionLabel = new Label($"Version {version}") { X = 2, Y = 2 };
        var descLabel = new Label(Description) { X = 2, Y = 4 };
        var copyrightLabel = new Label(Copyright) { X = 2, Y = 7 };
        var repoLabel = new Label(Repository) { X = 2, Y = 8 };
        var okButton = new Button("OK") { X = Pos.Center(), Y = 11 };

        okButton.Clicked += () => Application.RequestStop();

        dialog.Add(nameLabel, versionLabel, descLabel, copyrightLabel, repoLabel, okButton);
        okButton.SetFocus();

        Application.Run(dialog);
    }

    private static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

    public static AboutDialog Create() => new();
}
