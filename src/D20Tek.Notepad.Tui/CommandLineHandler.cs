using System.Text;

namespace D20Tek.Notepad.Tui;

internal static class CommandLineHandler
{
    public static IDocument RetrieveDocument(string[] args)
    {
        IDocument document;
        var docFactory = new DocumentFactory();

        if (args.Length > 0)
        {
            var filename = args[0];
            if (File.Exists(filename))
                document = docFactory.Load(filename);
            else
            {
                MessageBox.ErrorQuery("Error", $"File not found:\n{filename}", "OK");
                document = docFactory.Create(new([new(string.Empty)], Encoding.UTF8, LineEndingStyle.CRLF));
            }
        }
        else
        {
            document = docFactory.Create(new([new(string.Empty)], Encoding.UTF8, LineEndingStyle.CRLF));
        }
        return document;
    }
}
