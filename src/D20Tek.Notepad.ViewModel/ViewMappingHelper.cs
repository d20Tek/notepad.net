using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel;

internal static class ViewMappingHelper
{
    public static ViewPosition ToWrappedViewPosition(
        TextPosition docPos,
        IReadOnlyList<ViewLine> visibleLines,
        int firstVisibleVisualLine)
    {
        ArgumentNullException.ThrowIfNull(visibleLines);

        for (int viewLineIndex = 0; viewLineIndex < visibleLines.Count; viewLineIndex++)
        {
            var viewLine = visibleLines[viewLineIndex];
            if (viewLine.DocumentLineIndex != docPos.Line) continue;

            int segmentStart = viewLine.SegmentStartColumn;
            int segmentEnd = segmentStart + viewLine.Text.Length;

            // Check if the document column falls within this segment
            if (docPos.Column >= segmentStart && docPos.Column <= segmentEnd)
            {
                int columnInSegment = docPos.Column - segmentStart;
                return new ViewPosition(viewLineIndex, columnInSegment);
            }

            if (docPos.Column < segmentStart) return new ViewPosition(viewLineIndex, 0);
        }

        // If not found in visible lines, return position relative to last visible line
        if (visibleLines.Count > 0)
        {
            var lastLine = visibleLines[^1];
            if (docPos.Line == lastLine.DocumentLineIndex)
            {
                return new ViewPosition(visibleLines.Count - 1, lastLine.Text.Length);
            }
            else if (docPos.Line > lastLine.DocumentLineIndex)
            {
                return new ViewPosition(visibleLines.Count - 1, lastLine.Text.Length);
            }
        }

        // Document position is above visible area
        return new ViewPosition(0, 0);
    }

    public static TextPosition FromWrappedViewPosition(
        ViewPosition viewPos,
        IReadOnlyList<ViewLine> visibleLines,
        int firstVisibleVisualLine)
    {
        ArgumentNullException.ThrowIfNull(visibleLines);

        if (viewPos.LineIndex < 0 || viewPos.LineIndex >= visibleLines.Count)
        {
            // Return a safe default - first line, column 0
            return visibleLines.Count > 0
                ? new TextPosition(visibleLines[0].DocumentLineIndex, visibleLines[0].SegmentStartColumn)
                : new TextPosition(0, 0);
        }

        var viewLine = visibleLines[viewPos.LineIndex];
        int documentLine = viewLine.DocumentLineIndex;
        int documentColumn = viewLine.SegmentStartColumn + Math.Min(viewPos.Column, viewLine.Text.Length);

        return new TextPosition(documentLine, documentColumn);
    }

    public static int GetViewLineIndexForDocumentPosition(TextPosition docPos, IReadOnlyList<ViewLine> visibleLines)
    {
        ArgumentNullException.ThrowIfNull(visibleLines);

        for (int i = 0; i < visibleLines.Count; i++)
        {
            var viewLine = visibleLines[i];
            if (viewLine.DocumentLineIndex != docPos.Line) continue;

            int segmentStart = viewLine.SegmentStartColumn;
            int segmentEnd = segmentStart + viewLine.Text.Length;

            if (docPos.Column >= segmentStart && docPos.Column <= segmentEnd)
            {
                return i;
            }
        }

        return -1;
    }

    public static bool IsDocumentPositionVisible(TextPosition docPos, IReadOnlyList<ViewLine> visibleLines) =>
        GetViewLineIndexForDocumentPosition(docPos, visibleLines) >= 0;
}
