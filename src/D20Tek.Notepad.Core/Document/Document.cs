using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.Core.Document;

internal sealed class Document : IDocument
{
    private readonly List<TextLine> _lines;

    public IReadOnlyList<TextLine> Lines => _lines;

    public Encoding Encoding { get; private set; }

    public LineEndingStyle LineEndingStyle { get; private set; }

    public bool IsModified { get; private set; }

    public int LineCount => _lines.Count;

    public Document(IEnumerable<TextLine> lines)
        : this(lines, Encoding.UTF8, LineEndingStyle.CRLF)
    {
    }

    public Document(IEnumerable<TextLine> lines, Encoding encoding, LineEndingStyle lineEndingStyle)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(encoding);

        Encoding = encoding;
        LineEndingStyle = lineEndingStyle;

        _lines = [.. lines];
        EnsureSingleLine();

        IsModified = false;
    }

    public void ReplaceLines(int startIndex, int count, IEnumerable<TextLine> newLines)
    {
        ArgumentNullException.ThrowIfNull(newLines);
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, _lines.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, _lines.Count);

        _lines.RemoveRange(startIndex, count);
        _lines.InsertRange(startIndex, newLines);

        EnsureSingleLine();
        Modify();
    }

    public void SetEncoding(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        Encoding = encoding;
        Modify();
    }

    public void SetLineEndingStyle(LineEndingStyle style)
    {
        LineEndingStyle = style;
        Modify();
    }

    private void Modify() => IsModified = true;

    private void EnsureSingleLine()
    {
        if (_lines.Count == 0) _lines.Add(new TextLine(string.Empty));
    }
}
