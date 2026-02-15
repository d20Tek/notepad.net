using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

internal static class LineEditingService
{
    public static (TextLine newLine, int newColumn) Insert(TextLine line, int column, string text)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(column, line.Content.Length);

        string before = line.Content[..column];
        string after = line.Content[column..];

        string newContent = before + text + after;
        int newColumn = column + text.Length;

        return (new TextLine(newContent), newColumn);
    }

    public static (TextLine newLine, int newColumn) Delete(TextLine line, int column)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(column, line.Content.Length);

        if (column == 0) return (line, 0);

        string before = line.Content[..(column - 1)];
        string after = line.Content[column..];

        string newContent = before + after;
        int newColumn = column - 1;

        return (new TextLine(newContent), newColumn);
    }

    public static (TextLine left, TextLine right) Split(TextLine line, int column)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(column, line.Content.Length);

        string left = line.Content[..column];
        string right = line.Content[column..];

        return (new TextLine(left), new TextLine(right));
    }

    public static TextLine Merge(TextLine left, TextLine right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return new TextLine(left.Content + right.Content);
    }
}
