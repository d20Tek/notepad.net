using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.ViewModel;

internal static class WordWrapHelper
{
    public static int GetTotalVisualLineCount(IDocument document, int viewportWidth)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (viewportWidth <= 0 || document.LineCount == 0) return document.LineCount;

        int totalVisualLines = 0;
        foreach (var line in document.Lines)
        {
            var segments = WordWrapCalculator.WrapLine(line.Content, viewportWidth);
            totalVisualLines += segments.Count;
        }

        return totalVisualLines;
    }

    public static int GetVisualLineIndexForDocumentLine(IDocument document, int viewportWidth, int documentLine)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentOutOfRangeException.ThrowIfNegative(documentLine);
        if (viewportWidth <= 0 || document.LineCount == 0) return documentLine;

        int visualLineIndex = 0;
        int linesToProcess = Math.Min(documentLine, document.LineCount);

        for (int i = 0; i < linesToProcess; i++)
        {
            var segments = WordWrapCalculator.WrapLine(document.Lines[i].Content, viewportWidth);
            visualLineIndex += segments.Count;
        }

        return visualLineIndex;
    }

    public static int GetVisualLineIndexForPosition(IDocument document, int viewportWidth, int documentLine, int column)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (viewportWidth <= 0 || document.LineCount == 0 || documentLine < 0 || documentLine >= document.LineCount)
            return documentLine;

        int visualLineIndex = GetVisualLineIndexForDocumentLine(document, viewportWidth, documentLine);

        var lineText = document.Lines[documentLine].Content;
        var segments = WordWrapCalculator.WrapLine(lineText, viewportWidth);

        // Find which segment contains the column
        for (int i = 0; i < segments.Count - 1; i++)
        {
            int segmentEnd = segments[i].StartColumn + segments[i].Length;
            if (column <= segmentEnd)
            {
                return visualLineIndex + i;
            }
        }

        return visualLineIndex + segments.Count - 1;
    }
}
