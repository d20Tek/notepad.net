namespace D20Tek.Notepad.Core.Primitives;

public readonly record struct TextPosition(int Line, int Column) : IComparable<TextPosition>
{
    public int CompareTo(TextPosition other)
    {
        var lineCompare = Line.CompareTo(other.Line);
        return lineCompare != 0 ? lineCompare : Column.CompareTo(other.Column);
    }

    public override string ToString() => $"({Line}, {Column})";

    public static TextPosition Min(TextPosition a, TextPosition b) => a.CompareTo(b) <= 0 ? a : b;

    public static TextPosition Max(TextPosition a, TextPosition b) => a.CompareTo(b) >= 0 ? a : b;

    public TextPosition AdvanceColumn(int delta) => new(Line, Column + delta);
}
