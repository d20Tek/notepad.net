namespace D20Tek.Notepad.Core.Search;

public sealed record SearchOptions
{
    public static readonly SearchOptions Default = new();

    public bool CaseSensitive { get; init; } = false;

    public bool WrapAround { get; init; } = true;
}
