using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Storage;

public sealed class SimpleTextStorage : ITextStorageStrategy
{
    public DocumentData Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var encodingResult = stream.DetectEncoding();
        string text = ReadStreamToEnd(stream, encodingResult);
        var lineEnding = LineEndingStyleExtensions.FromString(text);
        var lines = SplitIntoLines(text);

        return new DocumentData(lines, encodingResult.Encoding, lineEnding);
    }

    public void Save(Stream stream, IDocument document)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(document);

        string newline = document.LineEndingStyle.ToLineEndingString();
        string text = string.Join(newline, document.Lines.Select(l => l.Content));

        using var writer = new StreamWriter(stream, document.Encoding, leaveOpen: true);
        writer.Write(text);
        writer.Flush();
    }

    private static string ReadStreamToEnd(Stream stream, EncodingDetector.EncodingResult encodingResult)
    {
        stream.Position = encodingResult.PreambleLength;
        using var reader = new StreamReader(
            stream,
            encodingResult.Encoding,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        return reader.ReadToEnd();
    }

    private static List<TextLine> SplitIntoLines(string text)
    {
        if (string.IsNullOrEmpty(text)) return [TextLine.Empty];
        
        var result = new List<TextLine>();
        foreach (var line in text.AsSpan().EnumerateLines())
        {
            result.Add(new TextLine(line.ToString()));
        }

        return result;
    }
}
