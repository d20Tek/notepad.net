using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel;

internal sealed class VisualLineIndex
{
    private int[] _segmentCounts = [];
    private int _viewportWidth;
    private int _lineCount;

    public int TotalVisualLineCount { get; private set; }

    public void Rebuild(IDocument document, int viewportWidth)
    {
        _viewportWidth = viewportWidth;
        _lineCount = document.LineCount;

        if (_segmentCounts.Length < _lineCount)
            _segmentCounts = new int[_lineCount];

        TotalVisualLineCount = 0;
        for (int i = 0; i < _lineCount; i++)
        {
            int count = WordWrapCalculator.CountSegments(document.Lines[i].Content, viewportWidth);
            _segmentCounts[i] = count;
            TotalVisualLineCount += count;
        }
    }

    public bool NeedsRebuild(int documentLineCount, int viewportWidth) =>
        _lineCount != documentLineCount || _viewportWidth != viewportWidth;

    public void UpdateLine(IDocument document, int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= _lineCount) return;

        int oldCount = _segmentCounts[lineIndex];
        int newCount = WordWrapCalculator.CountSegments(document.Lines[lineIndex].Content, _viewportWidth);

        if (oldCount != newCount)
        {
            _segmentCounts[lineIndex] = newCount;
            TotalVisualLineCount += (newCount - oldCount);
        }
    }

    public int GetVisualLineIndexForDocumentLine(int documentLine)
    {
        int visualLine = 0;
        int limit = Math.Min(documentLine, _lineCount);
        for (int i = 0; i < limit; i++)
            visualLine += _segmentCounts[i];
        return visualLine;
    }

    public (int DocLine, int SegmentOffset) FindDocumentLineForVisualLine(int visualLine)
    {
        int accumulated = 0;
        for (int i = 0; i < _lineCount; i++)
        {
            int next = accumulated + _segmentCounts[i];
            if (next > visualLine)
                return (i, visualLine - accumulated);
            accumulated = next;
        }

        return (_lineCount > 0 ? _lineCount - 1 : 0, 0);
    }
}
