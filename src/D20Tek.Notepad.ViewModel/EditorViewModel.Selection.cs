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
        var (selStart, selEnd) = anchor.CompareTo(caret) <= 0 ? (anchor, caret) : (caret, anchor);

        if (selEnd.Line < docLine || selStart.Line > docLine) return null;  // selection doesn't touch this document line

        int selStartCol = selStart.Line == docLine ? selStart.Column : 0;
        int selEndCol = selEnd.Line == docLine ? selEnd.Column : int.MaxValue;

        if (selStartCol >= segmentEnd) return null;
        if (selEndCol <= segmentStart) return null;

        // Calculate the selection within this segment
        int localStart = Math.Max(0, selStartCol - segmentStart);
        int localEnd = Math.Min(textLength, selEndCol - segmentStart);

        return new SelectionSegment(localStart, localEnd);
    }

    public void ExtendSelectionTo(int line, int column)
    {
        Session.Caret = Session.ClampToDocument(line, column);
        // IMPORTANT: Do NOT modify session.Anchor here. Anchor was set when selection began.
    }

    private static bool SelectionChangedNeeded(SelectionViewRange? oldSel, SelectionViewRange? newSel)
    {
        if (oldSel is null && newSel is null) return false;
        if (oldSel is null || newSel is null) return true;

        return !oldSel.Value.Equals(newSel.Value);
    }
}
