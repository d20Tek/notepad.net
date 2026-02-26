using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Notepad.Core.Search;

public sealed class SearchService
{
    public SearchResult FindNext(IDocument document, string searchTerm, TextPosition fromPosition, SearchOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(searchTerm);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrEmpty(searchTerm) || document.LineCount == 0) return SearchResult.NotFound;

        var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int startLine = Math.Clamp(fromPosition.Line, 0, document.LineCount - 1);
        int startColumn = Math.Clamp(fromPosition.Column, 0, document.Lines[startLine].Content.Length);

        // Search from current position to end of document
        var result = SearchForward(document, searchTerm, startLine, startColumn, document.LineCount - 1, comparison);
        if (result.Found) return result;

        // If wrap around is enabled, search from beginning to current position
        if (options.WrapAround && (startLine > 0 || startColumn > 0))
        {
            result = SearchForward(document, searchTerm, 0, 0, startLine, comparison);
            if (HasValidSearchResult(fromPosition, result)) return result;
        }

        return SearchResult.NotFound;
    }

    [ExcludeFromCodeCoverage]
    private static bool HasValidSearchResult(TextPosition fromPosition, SearchResult result) =>
        result.Found && result.Start.CompareTo(fromPosition) < 0;

    public SearchResult FindPrevious(
        IDocument document,
        string searchTerm,
        TextPosition fromPosition,
        SearchOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(searchTerm);
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrEmpty(searchTerm) || document.LineCount == 0) return SearchResult.NotFound;

        var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int startLine = Math.Clamp(fromPosition.Line, 0, document.LineCount - 1);
        int startColumn = Math.Clamp(fromPosition.Column, 0, document.Lines[startLine].Content.Length);

        // Search backward from current position to beginning
        var result = SearchBackward(document, searchTerm, startLine, startColumn, comparison);
        if (result.Found) return result;

        // If wrap around is enabled, search from end to current position
        if (options.WrapAround)
        {
            int lastLine = document.LineCount - 1;
            int lastColumn = document.Lines[lastLine].Content.Length;
            result = SearchBackward(document, searchTerm, lastLine, lastColumn, comparison);

            if (result.Found && result.Start.CompareTo(fromPosition) > 0) return result;
        }

        return SearchResult.NotFound;
    }

    private static SearchResult SearchForward(
        IDocument document,
        string searchTerm,
        int startLine,
        int startColumn,
        int endLine,
        StringComparison comparison)
    {
        for (int line = startLine; line <= endLine && line < document.LineCount; line++)
        {
            string content = document.Lines[line].Content;
            int searchStartColumn = (line == startLine) ? startColumn : 0;

            if (searchStartColumn >= content.Length) continue;

            int index = content.IndexOf(searchTerm, searchStartColumn, comparison);
            if (index >= 0)
            {
                var start = new TextPosition(line, index);
                var end = new TextPosition(line, index + searchTerm.Length);
                return SearchResult.Match(start, end);
            }
        }

        return SearchResult.NotFound;
    }

    private static SearchResult SearchBackward(
        IDocument document,
        string searchTerm,
        int startLine,
        int startColumn,
        StringComparison comparison)
    {
        for (int line = startLine; line >= 0; line--)
        {
            string content = document.Lines[line].Content;
            int searchEndColumn = (line == startLine) ? Math.Min(startColumn, content.Length) : content.Length;

            if (searchEndColumn <= 0) continue;

            // Search backward by finding last occurrence before the end column
            int index = content.LastIndexOf(searchTerm, searchEndColumn - 1, searchEndColumn, comparison);
            if (index >= 0)
            {
                var start = new TextPosition(line, index);
                var end = new TextPosition(line, index + searchTerm.Length);
                return SearchResult.Match(start, end);
            }
        }

        return SearchResult.NotFound;
    }
}
