using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed partial class EditorSession
{
    public bool HasSelection => !Caret.Equals(Anchor);

    public TextRange GetSelectionRange() => new TextRange(Anchor, Caret).Normalized();

    public string GetSelectedText() => !HasSelection ? string.Empty : GetTextInRange(GetSelectionRange());

    public void SelectAll()
    {
        Anchor = new TextPosition(0, 0);
        var lastLine = Document.Lines[^1];
        Caret = new TextPosition(Document.Lines.Count - 1, lastLine.Content.Length);
    }

    public void ClearSelection() => Anchor = Caret;

    internal string GetTextInRange(TextRange range)
    {
        range = range.Normalized();

        if (range.Start.Line == range.End.Line)
        {
            var line = Document.Lines[range.Start.Line];
            return line.Content[range.Start.Column..range.End.Column];
        }

        var parts = new List<string>();

        // first line
        var first = Document.Lines[range.Start.Line];
        parts.Add(first.Content.Substring(range.Start.Column));

        // middle lines
        for (int i = range.Start.Line + 1; i < range.End.Line; i++)
            parts.Add(Document.Lines[i].Content);

        // last line
        var last = Document.Lines[range.End.Line];
        parts.Add(last.Content.Substring(0, range.End.Column));

        return string.Join(Environment.NewLine, parts);
    }
}
