namespace D20Tek.Notepad.ViewModel;

public sealed class ViewLine
{
    public int DocumentLineIndex { get; }

    public string Text { get; }

    public ViewLine(int documentLineIndex, string text)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(documentLineIndex);
        ArgumentNullException.ThrowIfNull(text);

        DocumentLineIndex = documentLineIndex;
        Text = text;
    }

    public override string ToString() => Text;
}
