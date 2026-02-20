using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.Core.Storage;
using System.Text;

namespace D20Tek.Notepad.Core.Document;

public sealed class DocumentFactory : IDocumentFactory
{
    public IDocument Create(DocumentData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(data.Lines);
        ArgumentNullException.ThrowIfNull(data.Encoding);

        return new Document(EnsureTextLines(data), data.Encoding, data.LineEndingStyle);
    }

    public IDocument Empty { get; } = new Document([TextLine.Empty], Encoding.UTF8, LineEndingStyle.CRLF);

    public IDocument Load(string filePath)
    {
        using var stream = File.OpenRead(filePath);

        var storage = new SimpleTextStorage();
        DocumentData data = storage.Load(stream);

        return Create(data);
    }

    private static List<TextLine> EnsureTextLines(DocumentData data) =>
        data.Lines.Count == 0 ? [TextLine.Empty] : data.Lines;

}
