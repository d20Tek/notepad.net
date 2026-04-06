global using D20Tek.Notepad.Core.Document;
global using D20Tek.Notepad.Core.Editing;
global using D20Tek.Notepad.Core.Primitives;
global using D20Tek.Notepad.ViewModel;
global using Terminal.Gui;
using D20Tek.Notepad.Tui.Commands;
using D20Tek.Notepad.Tui.Menus;

namespace D20Tek.Notepad.Tui;

class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Application.Init();
        var top = Application.Top;

        var settingsService = new SettingsService();
        var settings = settingsService.Load();

        var (document, filePath) = CommandLineHandler.GetDocumentAndPath(args);
        var session = new EditorSession(document);
        var clipboardService = new TerminalGuiClipboardService();
        var viewModel = new EditorViewModel(session, new EditorCommandService(session), settings, clipboardService);

        // Set file path if loaded from command line
        if (!string.IsNullOrEmpty(filePath))
        {
            viewModel.SetFilePath(filePath);
        }

        // Initialize title bar manager (sets Console.Title)
        var titleBarManager = new TitleBarManager(viewModel);

        // Create editor and status bar views
        var editorView = EditorViewFactory.Create(viewModel, settings.StatusBarEnabled);
        var statusBarView = new StatusBarView(viewModel) { Visible = settings.StatusBarEnabled };

        // Save settings when values change.
        viewModel.WordWrapChanged += (_) => settingsService.Save(viewModel.Settings);
        viewModel.LineNumbersChanged += (_) => settingsService.Save(viewModel.Settings);
        viewModel.StatusBarChanged += (enabled) =>
        {
            statusBarView.Visible = enabled;
            editorView.Height = enabled ? Dim.Fill() - 1 : Dim.Fill();
            Application.Top.LayoutSubviews();
            Application.Top.SetNeedsDisplay();
            settingsService.Save(viewModel.Settings);
        };
        editorView.ZoomRequested += () => ZoomCommands.ShowHint(viewModel, statusBarView);

        var menuBar = MenuBuilder.Build(viewModel, statusBarView);
        viewModel.RecentFilesChanged += (_) =>
        {
            settingsService.Save(viewModel.Settings);
            top.Remove(menuBar);
            menuBar = MenuBuilder.Build(viewModel, statusBarView);
            top.Add(menuBar);
            top.SetNeedsDisplay();
        };

        top.Add(menuBar);
        top.Add(editorView);
        top.Add(statusBarView);

        Application.Run();

        viewModel.DisposeTimers();
        titleBarManager.Dispose();
        statusBarView.Dispose();
        editorView.Dispose();
        Application.Shutdown();
    }
}

