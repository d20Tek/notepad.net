global using D20Tek.Notepad.Core.Document;
global using D20Tek.Notepad.Core.Editing;
global using D20Tek.Notepad.Core.Primitives;
global using D20Tek.Notepad.ViewModel;
global using Terminal.Gui;
using D20Tek.Notepad.Tui.Menus;

namespace D20Tek.Notepad.Tui;

class Program
{
    static void Main(string[] args)
    {
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

        // Create editor view - starts at row 1 (below menu bar)
        var editorView = EditorViewFactory.Create(viewModel);

        // Save settings when word wrap changes
        viewModel.WordWrapChanged += (_) => settingsService.Save(viewModel.Settings);

        top.Add(MenuBuilder.Build(viewModel));
        top.Add(editorView);

        Application.Run();

        viewModel.DisposeTimers();
        titleBarManager.Dispose();
        Application.Shutdown();
    }
}

