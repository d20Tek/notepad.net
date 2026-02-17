using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel;

public static class ViewMapping
{
    public static ViewPosition DocumentToView(TextPosition docPos, int firstVisibleLine)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(docPos.Line, firstVisibleLine, nameof(docPos));
        int viewLine = docPos.Line - firstVisibleLine;
        return new ViewPosition(viewLine, docPos.Column);
    }

    public static TextPosition ViewToDocument(ViewPosition viewPos, int firstVisibleLine)
    {
        int docLine = viewPos.LineIndex + firstVisibleLine;
        return new TextPosition(docLine, viewPos.Column);
    }

    public static SelectionViewRange DocumentSelectionToView(
        TextPosition anchor,
        TextPosition caret,
        int firstVisibleLine,
        int visibleLineCount)
    {
        var start = ClampToViewport(anchor, firstVisibleLine, visibleLineCount);
        var end = ClampToViewport(caret, firstVisibleLine, visibleLineCount);
        return new SelectionViewRange(start, end).Normalize();
    }

    public static ViewPosition ClampToViewport(TextPosition docPos, int firstVisibleLine, int visibleLineCount)
    {
        int lastVisibleLine = firstVisibleLine + visibleLineCount - 1;

        int clampedLine = Math.Clamp(docPos.Line, firstVisibleLine, lastVisibleLine);
        int viewLine = clampedLine - firstVisibleLine;

        return new ViewPosition(viewLine, docPos.Column);
    }
}
