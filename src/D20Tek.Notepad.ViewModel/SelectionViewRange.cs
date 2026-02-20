namespace D20Tek.Notepad.ViewModel;

public readonly struct SelectionViewRange(ViewPosition start, ViewPosition end) : IEquatable<SelectionViewRange>
{
    public ViewPosition Start { get; } = start;

    public ViewPosition End { get; } = end;

    public bool IsEmpty => Start == End;

    public SelectionViewRange Normalize()
    {
        if (Start.LineIndex < End.LineIndex) return this;

        if (Start.LineIndex > End.LineIndex) return new SelectionViewRange(End, Start);

        // Same line: order by column
        return Start.Column <= End.Column ? this : new SelectionViewRange(End, Start);
    }

    public bool Equals(SelectionViewRange other) => Start.Equals(other.Start) && End.Equals(other.End);

    public override bool Equals(object? obj) => obj is SelectionViewRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Start, End);

    public static bool operator ==(SelectionViewRange left, SelectionViewRange right) => left.Equals(right);

    public static bool operator !=(SelectionViewRange left, SelectionViewRange right) => !left.Equals(right);

    public override string ToString() => $"{Start} -> {End}";
}
