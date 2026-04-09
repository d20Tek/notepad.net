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

    public static IDocument CreateEmpty() => new Document([TextLine.Empty], Encoding.UTF8, LineEndingStyle.CRLF);

    public IDocument Empty { get; } = new Document([TextLine.Empty], Encoding.UTF8, LineEndingStyle.CRLF);

    public IDocument Load(
        string filePath,
        long largeFileThreshold = 10_485_760,
        ILoadProgress? progress = null,
        CancellationToken cancellation = default)
    {
        var fileLength = new FileInfo(filePath).Length;

        DocumentData data;
        if (fileLength < largeFileThreshold)
        {
            using var stream = File.OpenRead(filePath);
            data = new SimpleTextStorage().Load(stream);
        }
        else
        {
            data = new LargeTextStorage().Load(filePath, progress, cancellation);
        }

        return Create(data);
    }

    public void Save(IDocument document, string filePath)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = File.Create(filePath);
        var storage = new SimpleTextStorage();
        storage.Save(stream, document);
    }

    private static List<TextLine> EnsureTextLines(DocumentData data) =>
        data.Lines.Count == 0 ? [TextLine.Empty] : data.Lines;
}
