using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private IDocument? _cachedDocument;
    private Encoding? _cachedDocumentEncoding;
    private int _cachedCharVersion = -1;
    private int _cachedTotalChars = 0;
    private string _cachedEncoding = StatusDetails.Empty.DocumentEncoding;
    private string _cachedLineEnding = StatusDetails.Empty.LineEndingStyle;

    internal StatusDetails BuildStatus()
    {
        bool documentChanged = !ReferenceEquals(_cachedDocument, Session.Document);

        if (documentChanged)
        {
            _cachedDocument = Session.Document;
            _cachedDocumentEncoding = Session.Document.Encoding;
            _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
            _cachedLineEnding = GetLineEndingDisplay(Session.Document.LineEndingStyle);
            _cachedCharVersion = -1;
        }
        else if (!ReferenceEquals(_cachedDocumentEncoding, Session.Document.Encoding))
        {
            _cachedDocumentEncoding = Session.Document.Encoding;
            _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
        }

        int currentVersion = Session.UndoStack.Version;
        if (_cachedCharVersion != currentVersion || Session.UndoStack.HasPendingGroupOperations)
        {
            _cachedTotalChars = Session.Document.Lines.Sum(l => l.Content.Length) + (Session.Document.LineCount - 1);
            _cachedCharVersion = currentVersion;
        }

        return new StatusDetails(
            CurrentLine: Session.Caret.Line + 1,
            CurrentColumn: Session.Caret.Column + 1,
            TotalLines: Session.Document.LineCount,
            TotalCharacters: _cachedTotalChars,
            DocumentEncoding: _cachedEncoding,
            LineEndingStyle: _cachedLineEnding);
    }

    private static string GetEncodingDisplay(Encoding encoding)
    {
        if (encoding.WebName == "utf-8")
            return encoding.GetPreamble().Length > 0 ? "UTF-8 BOM" : "UTF-8";

        return encoding.WebName.ToLowerInvariant() switch
        {
            "utf-16" => "UTF-16 LE",
            "utf-16be" => "UTF-16 BE",
            var name when !string.IsNullOrEmpty(name) => name.ToUpperInvariant(),
            _ => "Unknown"
        };
    }

    private static string GetLineEndingDisplay(LineEndingStyle style) => style switch
    {
        LineEndingStyle.CRLF => "CRLF",
        LineEndingStyle.CR => "CR",
        _ => "LF"
    };
}
