using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public SelectionSegment? GetSelectionSegmentForLine(int viewLineIndex)
    {
        if (SelectionViewRange is not { } sel) return null;
        if (viewLineIndex < 0 || viewLineIndex >= VisibleLines.Count) return null;
        if (viewLineIndex < sel.Start.LineIndex || viewLineIndex > sel.End.LineIndex) return null;

        var viewLine = VisibleLines[viewLineIndex];
        var textLength = viewLine.Text.Length;
        var (startLine, endLine) = (sel.Start.LineIndex, sel.End.LineIndex);
        var (startCol, endCol) = (Math.Max(0, sel.Start.Column), Math.Max(0, sel.End.Column));

        // Clamp columns to segment length
        if (viewLineIndex == startLine)
        {
            startCol = Math.Min(startCol, textLength);
        }
        if (viewLineIndex == endLine)
        {
            endCol = Math.Min(endCol, textLength);
        }

        return (viewLineIndex == startLine, viewLineIndex == endLine) switch
        {
            (true, true)  => new SelectionSegment(startCol, Math.Max(startCol, endCol)),   // single line
            (true, false) => new SelectionSegment(startCol, textLength),                   // first line
            (false, true) => new SelectionSegment(0, endCol),                              // last line
            _             => new SelectionSegment(0, textLength)                           // middle line
        };
    }

    public SelectionSegment? GetSelectionSegmentForWrappedLine(int viewLineIndex)
    {
        if (SelectionViewRange is not { } sel) return null;
        if (viewLineIndex < 0 || viewLineIndex >= VisibleLines.Count) return null;

        var viewLine = VisibleLines[viewLineIndex];
        var textLength = viewLine.Text.Length;

        // In word wrap mode, we need to check if the document selection intersects this segment
        if (!Settings.WordWrapEnabled) return GetSelectionSegmentForLine(viewLineIndex);

        // Get the document range for this view line segment
        int docLine = viewLine.DocumentLineIndex;
        int segmentStart = viewLine.SegmentStartColumn;
        int segmentEnd = segmentStart + textLength;

        // Get document selection range (normalized)
        var anchor = Session.Anchor;
        var caret = Session.Caret;
        var (selStart, selEnd) = GetSelectionStartEnd(anchor, caret);

        if (IsInvalidDocLine(docLine, selStart, selEnd)) return null;  // selection doesn't touch this document line

        int selStartCol = selStart.Line == docLine ? selStart.Column : 0;
        int selEndCol = selEnd.Line == docLine ? selEnd.Column : int.MaxValue;

        if (selStartCol >= segmentEnd) return null;
        if (selEndCol <= segmentStart) return null;

        // Calculate the selection within this segment
        int localStart = Math.Max(0, selStartCol - segmentStart);
        int localEnd = Math.Min(textLength, selEndCol - segmentStart);

        return new SelectionSegment(localStart, localEnd);
    }

    [ExcludeFromCodeCoverage]
    private static (TextPosition, TextPosition) GetSelectionStartEnd(TextPosition anchor, TextPosition caret) =>
        anchor.CompareTo(caret) <= 0 ? (anchor, caret) : (caret, anchor);

    [ExcludeFromCodeCoverage]
    private static bool IsInvalidDocLine(int docLine, TextPosition selStart, TextPosition selEnd) =>
        selEnd.Line < docLine || selStart.Line > docLine;

    public void ExtendSelectionTo(int line, int column)
    {
        Session.Caret = Session.ClampToDocument(line, column);
        // IMPORTANT: Do NOT modify session.Anchor here. Anchor was set when selection began.
        Refresh();
    }

    private static bool SelectionChangedNeeded(SelectionViewRange? oldSel, SelectionViewRange? newSel)
    {
        if (oldSel is null && newSel is null) return false;
        if (oldSel is null || newSel is null) return true;

        return !oldSel.Value.Equals(newSel.Value);
    }

    public void SelectWordAt(int line, int column)
    {
        if (line < 0 || line >= Session.Document.LineCount) return;

        var content = Session.Document.Lines[line].Content;
        if (content.Length == 0) return;

        int clampedColumn = Math.Clamp(column, 0, content.Length - 1);

        // Find word boundaries - word is alphanumeric characters
        int start = clampedColumn;
        int end = clampedColumn;

        // If we clicked on a non-word character, select just that character
        if (!IsWordChar(content[clampedColumn]))
        {
            Session.Anchor = new TextPosition(line, clampedColumn);
            Session.Caret = new TextPosition(line, clampedColumn + 1);
            Refresh();
            return;
        }

        // Expand left to find start of word
        while (start > 0 && IsWordChar(content[start - 1]))
        {
            start--;
        }

        // Expand right to find end of word
        while (end < content.Length - 1 && IsWordChar(content[end + 1]))
        {
            end++;
        }

        Session.Anchor = new TextPosition(line, start);
        Session.Caret = new TextPosition(line, end + 1);
        Refresh();
    }

    public void SelectLineAt(int line)
    {
        if (line < 0 || line >= Session.Document.LineCount) return;

        int lineLength = Session.Document.Lines[line].Content.Length;
        Session.Anchor = new TextPosition(line, 0);
        Session.Caret = new TextPosition(line, lineLength);
        Refresh();
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';
}
