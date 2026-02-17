namespace D20Tek.Notepad.ViewModel;

public sealed class Viewport
{
    public int FirstVisibleLine { get; private set; }

    public int VisibleLineCount { get; private set; }

    public Viewport(int firstVisibleLine = 0, int visibleLineCount = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(firstVisibleLine);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleLineCount);

        FirstVisibleLine = firstVisibleLine;
        VisibleLineCount = visibleLineCount;
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
}
