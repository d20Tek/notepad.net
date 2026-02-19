using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.ViewModel;
using System.Text;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

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

        // Create your Terminal.Gui editor view
        var editorView = new EditorView(viewModel)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = new ColorScheme
            {
                Normal = new Attribute(Color.White, Color.Black),
                Focus = new Attribute(Color.White, Color.Black),
                HotNormal = new Attribute(Color.BrightYellow, Color.Black),
                HotFocus = new Attribute(Color.BrightYellow, Color.White),
                Disabled = new Attribute(Color.Gray, Color.Black)
            }
        };

        top.Add(editorView);

        Application.Run();
        Application.Shutdown();
    }
}
