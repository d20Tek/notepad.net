using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Search;

public readonly record struct SearchResult
{
    public static readonly SearchResult NotFound = new(false, new TextPosition(0, 0), new TextPosition(0, 0));

    public bool Found { get; }

    public TextPosition Start { get; }

    public TextPosition End { get; }

    public int Length => Found ? End.Column - Start.Column : 0;

    public SearchResult(bool found, TextPosition start, TextPosition end)
    {
        Found = found;
        Start = start;
        End = end;
    }

    public static SearchResult Match(TextPosition start, TextPosition end) => new(true, start, end);
}
