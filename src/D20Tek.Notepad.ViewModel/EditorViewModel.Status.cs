using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private IDocument? _cachedDocument;
    private int _cachedCharVersion = -1;
    private int _cachedTotalChars = 0;
    private string _cachedEncoding = StatusDetails.Empty.DocumentEncoding;
    private string _cachedLineEnding = StatusDetails.Empty.LineEndingStyle;

    internal StatusDetails BuildStatus()
    {
        if (!ReferenceEquals(_cachedDocument, Session.Document))
        {
            _cachedDocument = Session.Document;
            _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
            _cachedLineEnding = GetLineEndingDisplay(Session.Document.LineEndingStyle);
            _cachedCharVersion = -1;
        }

        int currentVersion = Session.UndoStack.Version;
        if (_cachedCharVersion != currentVersion || Session.UndoStack.HasPendingGroupOperations)
        {
            _cachedTotalChars = Session.Document.Lines.Sum(l => l.Content.Length) +
                (Session.Document.LineCount - 1);
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
        var webName = encoding?.WebName;
        return string.IsNullOrEmpty(webName) ? "Unknown" : webName.ToUpperInvariant();
    }

    private static string GetLineEndingDisplay(LineEndingStyle style) => style switch
    {
        LineEndingStyle.CRLF => "CRLF",
        LineEndingStyle.CR => "CR",
        _ => "LF"
    };
}
