namespace D20Tek.Notepad.ViewModel;

public sealed record MruList
{
    public const int MaxCapacity = 10;

    public static MruList Empty { get; } = new();

    public IReadOnlyList<string> Paths { get; init; } = [];

    public MruList Add(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var newPaths = Paths
            .Where(p => !string.Equals(p, path, StringComparison.OrdinalIgnoreCase))
            .Prepend(path)
            .Take(MaxCapacity)
            .ToList();

        return new MruList { Paths = newPaths };
    }
}
