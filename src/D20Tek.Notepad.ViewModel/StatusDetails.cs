using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.ViewModel;

public record StatusDetails(
    int CurrentLine,
    int CurrentColumn,
    int TotalLines,
    int TotalCharacters,
    string DocumentEncoding,
    string LineEndingStyle)
{
    public static StatusDetails Empty { get; } = new(1, 1, 1, 0, "UTF-8", "CRLF");

    internal static StatusDetails From(
        int caretLine,
        int caretColumn,
        IDocument document)
    {
        int totalChars = document.Lines.Sum(l => l.Content.Length) + (document.LineCount - 1);

        return new StatusDetails(
            CurrentLine: caretLine + 1,
            CurrentColumn: caretColumn + 1,
            TotalLines: document.LineCount,
            TotalCharacters: totalChars,
            DocumentEncoding: GetEncodingDisplay(document.Encoding),
            LineEndingStyle: GetLineEndingDisplay(document.LineEndingStyle));
    }

    private static string GetEncodingDisplay(Encoding encoding) => encoding.WebName.ToUpperInvariant();

    private static string GetLineEndingDisplay(LineEndingStyle style) => style switch
    {
        Core.Primitives.LineEndingStyle.CRLF => "CRLF",
        Core.Primitives.LineEndingStyle.CR => "CR",
        _ => "LF"
    };
}
