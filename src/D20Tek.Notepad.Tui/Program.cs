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

        var session = new EditorSession(CommandLineHandler.GetDocumentOrDefault(args));
        var viewModel = new EditorViewModel(session, new EditorCommandService(session), settings);

        // Save settings when word wrap changes
        viewModel.WordWrapChanged += (_) => settingsService.Save(viewModel.Settings);

        top.Add(MenuBuilder.Build(viewModel));
        top.Add(new HorizontalDivider(0, 1));
        top.Add(EditorViewFactory.Create(viewModel));

        Application.Run();
        Application.Shutdown();
    }
}
