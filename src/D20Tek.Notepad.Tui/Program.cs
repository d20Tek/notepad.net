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

        var document = CreateDocument(args);
        var session = new EditorSession(document);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session, new EditorNavigationService()));

        var editorView = EditorViewFactory.Create(viewModel);
        top.Add(editorView);

        Application.Run();
        Application.Shutdown();
    }

    private static IDocument CreateDocument(string[] args)
    {
        IDocument document;
        var docFactory = new DocumentFactory();

        if (args.Length > 0)
        {
            var filename = args[0];
            if (File.Exists(filename))
                document = docFactory.Load(filename);
            else
                document = docFactory.Create(
                    new([new($"File not found: {args[0]}")], Encoding.UTF8, LineEndingStyle.CRLF));
        }
        else
        { 
            document = docFactory.Create(new([new(string.Empty) ], Encoding.UTF8, LineEndingStyle.CRLF));
        }
        return document;   
    }
}
