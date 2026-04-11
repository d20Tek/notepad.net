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

    public int TotalCharacterCount { get; private set; }

    public int MaxLineLength { get; private set; }

    public Document(IEnumerable<TextLine> lines) : this(lines, Encoding.UTF8, LineEndingStyle.CRLF) { }

    public Document(IEnumerable<TextLine> lines, Encoding encoding, LineEndingStyle lineEndingStyle)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(encoding);

        Encoding = encoding;
        LineEndingStyle = lineEndingStyle;

        _lines = [.. lines];
        EnsureSingleLine();
        TotalCharacterCount = ComputeTotalChars();
        MaxLineLength = ComputeMaxLineLength();

        IsModified = false;
    }

    public void ReplaceLines(int startIndex, int count, IEnumerable<TextLine> newLines)
    {
        ArgumentNullException.ThrowIfNull(newLines);
        ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, _lines.Count);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + count, _lines.Count);

        int removedChars = SumContentLength(startIndex, count);
        int oldLineCount = _lines.Count;

        _lines.RemoveRange(startIndex, count);
        _lines.InsertRange(startIndex, newLines);

        EnsureSingleLine();

        int addedCount = _lines.Count - oldLineCount + count;
        int addedChars = SumContentLength(startIndex, addedCount);
        int lineCountDelta = _lines.Count - oldLineCount;

        TotalCharacterCount += (addedChars - removedChars) + lineCountDelta;
        UpdateMaxLineLength(startIndex, addedCount, removedChars >= MaxLineLength);
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
        if (_lines.Count == 0) _lines.Add(TextLine.Empty);
    }

    private int ComputeTotalChars()
    {
        int total = 0;
        for (int i = 0; i < _lines.Count; i++)
            total += _lines[i].Content.Length;

        return total + (_lines.Count - 1);
    }

    private int SumContentLength(int startIndex, int count)
    {
        int total = 0;
        for (int i = startIndex; i < startIndex + count; i++)
            total += _lines[i].Content.Length;

        return total;
    }

    private int ComputeMaxLineLength()
    {
        int max = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            if (_lines[i].Content.Length > max)
                max = _lines[i].Content.Length;
        }

        return max;
    }

    private void UpdateMaxLineLength(int startIndex, int addedCount, bool removedMaxLine)
    {
        int newMax = MaxLineLength;
        for (int i = startIndex; i < startIndex + addedCount; i++)
        {
            if (_lines[i].Content.Length > newMax)
                newMax = _lines[i].Content.Length;
        }

        if (newMax > MaxLineLength)
        {
            MaxLineLength = newMax;
        }
        else if (removedMaxLine)
        {
            MaxLineLength = ComputeMaxLineLength();
        }
    }
}
