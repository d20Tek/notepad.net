using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel;

public static class ViewMapping
{
    public static ViewPosition ToViewPosition(this TextPosition docPos, int firstVisibleLine)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(docPos.Line, firstVisibleLine, nameof(docPos));
        int viewLine = docPos.Line - firstVisibleLine;
        return new ViewPosition(viewLine, docPos.Column);
    }

    public static TextPosition ToDocumentPosition(this ViewPosition viewPos, int firstVisibleLine)
    {
        int docLine = viewPos.LineIndex + firstVisibleLine;
        return new TextPosition(docLine, viewPos.Column);
    }

    public static ViewPosition ClampToViewport(this TextPosition docPos, int firstVisibleLine, int visibleLineCount)
    {
        if (visibleLineCount <= 0)
            return new ViewPosition(0, docPos.Column);

        int lastVisibleLine = firstVisibleLine + visibleLineCount - 1;
        int clampedLine = Math.Clamp(docPos.Line, firstVisibleLine, lastVisibleLine);
        int viewLine = clampedLine - firstVisibleLine;

        return new ViewPosition(viewLine, docPos.Column);
    }

    public static SelectionViewRange? ToViewSelectionRange(
        this TextPosition anchor,
        TextPosition caret,
        int firstVisibleLine,
        int visibleLineCount)
    {
        // Normalize to get the actual start and end positions
        var (selStart, selEnd) = anchor.CompareTo(caret) <= 0 ? (anchor, caret) : (caret, anchor);
        int lastVisibleLine = firstVisibleLine + visibleLineCount - 1;

        // Check if selection intersects viewport at all
        if (selEnd.Line < firstVisibleLine || selStart.Line > lastVisibleLine)
            return null;

        var start = anchor.ClampToViewport(firstVisibleLine, visibleLineCount);
        var end = caret.ClampToViewport(firstVisibleLine, visibleLineCount);
        return new SelectionViewRange(start, end).Normalize();
    }

    // Backward compatibility - static method wrappers
    public static ViewPosition DocumentToView(TextPosition docPos, int firstVisibleLine) =>
        docPos.ToViewPosition(firstVisibleLine);

    public static TextPosition ViewToDocument(ViewPosition viewPos, int firstVisibleLine) =>
        viewPos.ToDocumentPosition(firstVisibleLine);

    public static SelectionViewRange? DocumentSelectionToView(
        TextPosition anchor,
        TextPosition caret,
        int firstVisibleLine,
        int visibleLineCount) =>
        anchor.ToViewSelectionRange(caret, firstVisibleLine, visibleLineCount);
}
