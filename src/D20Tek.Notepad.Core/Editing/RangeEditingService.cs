using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

internal static class RangeEditingService
{
    public static TextPosition ReplaceRange(IDocument document, TextRange range, string replacementText)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(replacementText);

        range = range.Normalized();

        int startLine = range.Start.Line;
        int startCol = range.Start.Column;
        int endLine = range.End.Line;
        int endCol = range.End.Column;

        // extract the first and last lines
        var firstLine = document.Lines[startLine];
        var lastLine = document.Lines[endLine];

        // split the first and last lines at the range boundaries
        var (leftPart, _) = LineEditingService.Split(firstLine, startCol);
        var (_, rightPart) = LineEditingService.Split(lastLine, endCol);

        // split replacement text into lines
        var replacementLines = SplitReplacementText(replacementText);

        // build the new lines to insert
        var newLines = BuildReplacementLines(leftPart, rightPart, replacementLines);

        // replace the affected lines in the document
        int lineCountToReplace = endLine - startLine + 1;
        document.ReplaceLines(startLine, lineCountToReplace, newLines);

        return ComputeNewCaret(startLine, leftPart, replacementLines);
    }

    private static List<TextLine> SplitReplacementText(string text)
    {
        // if no newlines, treat as a single line
        if (!text.Contains('\n')) return [new(text)];

        var lines = new List<TextLine>();
        int start = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                int end = (i > 0 && text[i - 1] == '\r') ? i - 1 : i;
                string segment = text[start..end];
                lines.Add(new TextLine(segment));
                start = i + 1;
            }
        }

        // final line, even if empty
        if (start <= text.Length) lines.Add(new TextLine(text[start..]));

        return lines;
    }

    private static List<TextLine> BuildReplacementLines(TextLine left, TextLine right, List<TextLine> replacement)
    {
        var result = new List<TextLine>();

        if (replacement.Count == 1)
        {
            // Single-line replacement: left + replacement + right
            string merged = left.Content + replacement[0].Content + right.Content;
            result.Add(new TextLine(merged));
            return result;
        }

        // multi-line replacement
        // first line: left + first replacement line
        result.Add(new TextLine(left.Content + replacement[0].Content));

        for (int i = 1; i < replacement.Count - 1; i++)
            result.Add(replacement[i]);

        // last line: last replacement line + right
        string last = replacement[^1].Content + right.Content;
        result.Add(new TextLine(last));

        return result;
    }

    private static TextPosition ComputeNewCaret(int startLine, TextLine leftPart, List<TextLine> replacement)
    {
        if (replacement.Count == 1)
        {
            // caret ends after left + replacement
            int col = leftPart.Content.Length + replacement[0].Content.Length;
            return new TextPosition(startLine, col);
        }

        // multi-line replacement: caret goes to end of last inserted line
        int newLine = startLine + replacement.Count - 1;
        int newCol = replacement[^1].Content.Length;
        return new TextPosition(newLine, newCol);
    }
}
