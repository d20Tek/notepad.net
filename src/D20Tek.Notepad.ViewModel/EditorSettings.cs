namespace D20Tek.Notepad.ViewModel;

public sealed record EditorSettings
{
    public static EditorSettings Default { get; } = new();

    public int MouseWheelScrollLines { get; init; } = 3;

    public int EdgeScrollSpeed { get; init; } = 3;

    public int HorizontalEdgeScrollAmount { get; init; } = 20;

    public int TabSize { get; init; } = 4;

    public bool WordWrapEnabled { get; init; } = false;
}
