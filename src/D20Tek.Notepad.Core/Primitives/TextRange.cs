namespace D20Tek.Notepad.Core.Primitives;

public readonly record struct TextRange(TextPosition Start, TextPosition End)
{
    public TextRange Normalized() => Start.CompareTo(End) <= 0 ? this : new TextRange(End, Start);

    public bool Contains(TextPosition position) => Start.CompareTo(position) <= 0 && position.CompareTo(End) <= 0;

    public bool IsEmpty => Start.Equals(End);
}
