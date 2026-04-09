namespace D20Tek.Notepad.ViewModel;

public sealed record EditorSettings
{
    public static EditorSettings Default { get; } = new();

    public int MouseWheelScrollLines { get; init; } = 3;

    public int EdgeScrollSpeed { get; init; } = 3;

    public int HorizontalEdgeScrollAmount { get; init; } = 20;

    public int TabSize { get; init; } = 4;

    public bool UseSpacesForTab { get; init; } = true;

    public bool WordWrapEnabled { get; init; } = false;

    public bool StatusBarEnabled { get; init; } = false;

    public bool LineNumbersEnabled { get; init; } = false;

    public IReadOnlyList<string> RecentFiles { get; init; } = [];

    public long LargeFileThresholdBytes { get; init; } = 10_485_760;
}
