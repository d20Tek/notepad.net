namespace D20Tek.Notepad.ViewModel;

public readonly struct ViewPosition : IEquatable<ViewPosition>
{
    public int LineIndex { get; }

    public int Column { get; }

    public ViewPosition(int lineIndex, int column)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(lineIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(column);

        LineIndex = lineIndex;
        Column = column;
    }

    public bool Equals(ViewPosition other) => LineIndex == other.LineIndex && Column == other.Column;

    public override bool Equals(object? obj) => obj is ViewPosition other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(LineIndex, Column);

    public static bool operator ==(ViewPosition left, ViewPosition right) => left.Equals(right);

    public static bool operator !=(ViewPosition left, ViewPosition right) => !left.Equals(right);

    public override string ToString() => $"({LineIndex}, {Column})";
}
