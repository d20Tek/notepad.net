namespace D20Tek.Notepad.Core.Primitives;

public sealed class TextLine(string content)
{
    public string Content { get; } = content;

    public static TextLine Empty { get; } = new(string.Empty);
}
