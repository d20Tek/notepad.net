namespace D20Tek.Notepad.ViewModel;

public sealed class Viewport
{
    public int FirstVisibleLine { get; private set; }

    public int VisibleLineCount { get; private set; }

    public int HorizontalOffset { get; private set; } = 0;

    public Viewport(int firstVisibleLine = 0, int visibleLineCount = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(firstVisibleLine);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleLineCount);

        FirstVisibleLine = firstVisibleLine;
        VisibleLineCount = visibleLineCount;
    }

    public void ResetViewport()
    {
        HorizontalOffset = 0;
        FirstVisibleLine = 0;
    }

    public void SetVisibleLineCount(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        VisibleLineCount = count;
    }

    public void ScrollLines(int delta, int totalDocumentLines)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalDocumentLines);

        int maxFirstLine = Math.Max(0, totalDocumentLines - VisibleLineCount);
        FirstVisibleLine = Math.Clamp(FirstVisibleLine + delta, 0, maxFirstLine);
    }

    public void ScrollPages(int deltaPages, int totalDocumentLines) =>
        ScrollLines(deltaPages * VisibleLineCount, totalDocumentLines);

    public void EnsureLineVisible(int lineIndex, int totalDocumentLines)
    {
        if (lineIndex < 0 || lineIndex >= totalDocumentLines) return;

        int lastVisibleLine = FirstVisibleLine + VisibleLineCount - 1;
        if (lineIndex < FirstVisibleLine)
        {
            FirstVisibleLine = lineIndex;
        }
        else if (lineIndex > lastVisibleLine)
        {
            FirstVisibleLine = lineIndex - VisibleLineCount + 1;
        }

        // clamp after adjustment
        int maxFirstLine = Math.Max(0, totalDocumentLines - VisibleLineCount);
        FirstVisibleLine = Math.Clamp(FirstVisibleLine, 0, maxFirstLine);
    }

    public bool IsLineVisible(int lineIndex) =>
        lineIndex >= FirstVisibleLine && lineIndex < FirstVisibleLine + VisibleLineCount;

    public void ScrollColumns(int delta)
    {
        int newOffset = HorizontalOffset + delta;
        HorizontalOffset = Math.Max(0, newOffset);
    }

    public void EnsureColumnVisible(int caretColumn, int viewportWidth)
    {
        if (viewportWidth <= 0) return;
        
        int left = HorizontalOffset;
        int right = HorizontalOffset + viewportWidth - 1;
        if (caretColumn < left)
        {
            HorizontalOffset = caretColumn;
        }
        else if (caretColumn > right)
        {
            HorizontalOffset = caretColumn - viewportWidth + 1;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(HorizontalOffset);
    }
}
