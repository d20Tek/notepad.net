namespace D20Tek.Notepad.Tui;

internal static class CommandLineHandler
{
    public static IDocument GetDocumentOrDefault(string[] args)
    {
        var docFactory = new DocumentFactory();
        IDocument document = docFactory.Empty;

        if (args.Length > 0)
        {
            var filename = args[0];
            if (File.Exists(filename))
                document = docFactory.Load(filename);
            else
                MessageBox.ErrorQuery("Error", $"File not found:\n{filename}", "OK");
        }

        return document;
    }
}
