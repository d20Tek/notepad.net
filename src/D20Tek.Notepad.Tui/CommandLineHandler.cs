namespace D20Tek.Notepad.Tui;

internal static class CommandLineHandler
{
    public static IDocument GetDocumentOrDefault(string[] args)
    {
        var (document, _) = GetDocumentAndPath(args);
        return document;
    }

    public static (IDocument document, string? filePath) GetDocumentAndPath(string[] args)
    {
        var docFactory = new DocumentFactory();

        if (args.Length > 0)
        {
            var filename = args[0];
            if (File.Exists(filename))
            {
                var document = docFactory.Load(filename);
                var fullPath = Path.GetFullPath(filename);
                return (document, fullPath);
            }
            else
            {
                MessageBox.ErrorQuery("Error", $"File not found:\n{filename}", "OK");
            }
        }

        return (docFactory.Empty, null);
    }
}
