using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.ViewModel;
using System.Text;
using Terminal.Gui;

namespace D20Tek.Notepad.Tui;

class Program
{
    static void Main(string[] args)
    {
        Application.Init();

        var top = Application.Top;

        // Create your editor session + viewmodel
        var docFactory = new DocumentFactory();
        var session = new EditorSession(docFactory.Create(new([new("Empty document loaded.")], Encoding.UTF8, LineEndingStyle.CRLF)));
        var viewModel = new EditorViewModel(session, new EditorCommandService(session, new EditorNavigationService()));

        var editorView = EditorViewFactory.Create(viewModel);
        top.Add(editorView);

        Application.Run();
        Application.Shutdown();
    }
}
