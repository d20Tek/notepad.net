using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private readonly SearchService _searchService = new();
    private string? _lastSearchTerm;
    private SearchOptions _searchOptions = SearchOptions.Default;

    public string? LastSearchTerm => _lastSearchTerm;

    public bool HasLastSearch => !string.IsNullOrEmpty(_lastSearchTerm);

    public SearchOptions SearchOptions
    {
        get => _searchOptions;
        set => _searchOptions = value ?? SearchOptions.Default;
    }

    public bool FindNext(string searchTerm)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);
        if (string.IsNullOrEmpty(searchTerm)) return false;

        _lastSearchTerm = searchTerm;
        var result = _searchService.FindNext(Session.Document, searchTerm, Session.Caret, _searchOptions);

        if (!result.Found) return false;

        SelectSearchResult(result);
        return true;
    }

    public bool FindPrevious(string searchTerm)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);
        if (string.IsNullOrEmpty(searchTerm)) return false;

        _lastSearchTerm = searchTerm;
        var result = _searchService.FindPrevious(Session.Document, searchTerm, Session.Caret, _searchOptions);

        if (!result.Found) return false;

        SelectSearchResult(result);
        return true;
    }

    public bool FindNextFromCurrent()
    {
        if (!HasLastSearch) return false;
        return FindNext(_lastSearchTerm!);
    }

    public bool FindPreviousFromCurrent() => (!HasLastSearch) ? false : FindPrevious(_lastSearchTerm!);

    public bool Replace(string searchTerm, string replacement)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);
        ArgumentNullException.ThrowIfNull(replacement);

        _lastSearchTerm = searchTerm;

        if (!Session.HasSelection) return FindNext(searchTerm);

        var selectedText = Session.GetSelectedText();
        var comparison = _searchOptions.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        if (!string.Equals(selectedText, searchTerm, comparison))
        {
            return FindNext(searchTerm);
        }

        EndTypingGroupIfNeeded();
        Commands.ReplaceSelection(replacement);
        CheckDirtyStateChanged();
        Refresh();

        return FindNext(searchTerm);
    }

    public int ReplaceAll(string searchTerm, string replacement)
    {
        ArgumentNullException.ThrowIfNull(searchTerm);
        ArgumentNullException.ThrowIfNull(replacement);

        if (string.IsNullOrEmpty(searchTerm)) return 0;

        _lastSearchTerm = searchTerm;
        EndTypingGroupIfNeeded();

        int count = 0;
        var startPosition = new TextPosition(0, 0);

        while (true)
        {
            var result = _searchService.FindNext(Session.Document, searchTerm, startPosition, _searchOptions with { WrapAround = false });
            if (!result.Found) break;

            Session.Anchor = result.Start;
            Session.Caret = result.End;
            Commands.ReplaceSelection(replacement);
            count++;

            startPosition = new TextPosition(result.Start.Line, result.Start.Column + replacement.Length);
        }

        if (count > 0)
        {
            CheckDirtyStateChanged();
            Refresh();
            EnsureCaretVisible();
        }

        return count;
    }

    public void GoToLine(int lineNumber)
    {
        int zeroBasedLine = Math.Clamp(lineNumber - 1, 0, Session.Document.LineCount - 1);

        Session.Caret = new TextPosition(zeroBasedLine, 0);
        Session.ClearSelection();

        Refresh();
        EnsureCaretVisible();
    }

    private void SelectSearchResult(SearchResult result)
    {
        Session.Anchor = result.Start;
        Session.Caret = result.End;
        Refresh();
        EnsureCaretVisible();
    }
}
