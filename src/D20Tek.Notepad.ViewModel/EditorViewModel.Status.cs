using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private IDocument? _cachedDocument;
    private Encoding? _cachedDocumentEncoding;
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
        }
        else if (!ReferenceEquals(_cachedDocumentEncoding, Session.Document.Encoding))
        {
            _cachedDocumentEncoding = Session.Document.Encoding;
            _cachedEncoding = GetEncodingDisplay(Session.Document.Encoding);
        }

        return new StatusDetails(
            CurrentLine: Session.Caret.Line + 1,
            CurrentColumn: Session.Caret.Column + 1,
            TotalLines: Session.Document.LineCount,
            TotalCharacters: Session.Document.TotalCharacterCount,
            DocumentEncoding: _cachedEncoding,
            LineEndingStyle: _cachedLineEnding,
            IsOverwriteMode: IsOverwriteMode);
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
