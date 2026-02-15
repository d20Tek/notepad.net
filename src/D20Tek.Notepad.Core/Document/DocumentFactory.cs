using D20Tek.Notepad.Core.Primitives;

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

    private static List<TextLine> EnsureTextLines(DocumentData data) =>
        data.Lines.Count == 0 ? [TextLine.Empty] : data.Lines;

}
