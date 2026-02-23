namespace D20Tek.Notepad.ViewModel;

public sealed class ViewLine
{
    public int DocumentLineIndex { get; }

    public int SegmentStartColumn { get; }

    public string Text { get; }

    public ViewLine(int documentLineIndex, string text)
        : this(documentLineIndex, 0, text) { }

    public ViewLine(int documentLineIndex, int segmentStartColumn, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(documentLineIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(segmentStartColumn);
        ArgumentNullException.ThrowIfNull(text);

        DocumentLineIndex = documentLineIndex;
        SegmentStartColumn = segmentStartColumn;
        Text = text;
    }

    public override string ToString() => Text;
}
