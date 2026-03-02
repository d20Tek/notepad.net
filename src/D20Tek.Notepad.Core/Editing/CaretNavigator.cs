using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed class CaretNavigator(EditorSession session)
{
    private readonly EditorSession _session = session;

    // Movement (resets selection by setting Anchor = Caret)
    public void MoveLeft() => Move(ComputeLeft, resetAnchor: true);

    public void MoveRight() => Move(ComputeRight, resetAnchor: true);

    public void MoveUp() => Move(ComputeUp, resetAnchor: true);

    public void MoveDown() => Move(ComputeDown, resetAnchor: true);

    public void MovePageUp(int pageHeight) => Move(() => ComputePageUp(pageHeight), resetAnchor: true);

    public void MovePageDown(int pageHeight) => Move(() => ComputePageDown(pageHeight), resetAnchor: true);

    public void MoveToLineStart() => Move(() => new TextPosition(_session.Caret.Line, 0), resetAnchor: true);

    public void MoveToLineEnd() => Move(() => new TextPosition(_session.Caret.Line, CurrentLineLength), resetAnchor: true);

    public void MoveToDocumentStart() => Move(() => new TextPosition(0, 0), resetAnchor: true);

    public void MoveToDocumentEnd() => Move(ComputeDocumentEnd, resetAnchor: true);

    // Word Navigation (resets selection)
    public void MoveWordLeft() => Move(ComputeWordLeft, resetAnchor: true);

    public void MoveWordRight() => Move(ComputeWordRight, resetAnchor: true);

    // Selection Extension (preserves anchor)
    public void ExtendLeft() => Move(ComputeLeft, resetAnchor: false);

    public void ExtendRight() => Move(ComputeRight, resetAnchor: false);

    public void ExtendUp() => Move(ComputeUp, resetAnchor: false);

    public void ExtendDown() => Move(ComputeDown, resetAnchor: false);

    public void ExtendPageUp(int pageHeight) => Move(() => ComputePageUp(pageHeight), resetAnchor: false);

    public void ExtendPageDown(int pageHeight) => Move(() => ComputePageDown(pageHeight), resetAnchor: false);

    public void ExtendToLineStart() => Move(() => new TextPosition(_session.Caret.Line, 0), resetAnchor: false);

    public void ExtendToLineEnd() => Move(() => new TextPosition(_session.Caret.Line, CurrentLineLength), resetAnchor: false);

    public void ExtendToDocumentStart() => Move(() => new TextPosition(0, 0), resetAnchor: false);

    public void ExtendToDocumentEnd() => Move(ComputeDocumentEnd, resetAnchor: false);

    // Word Selection Extension (preserves anchor)
    public void ExtendWordLeft() => Move(ComputeWordLeft, resetAnchor: false);

    public void ExtendWordRight() => Move(ComputeWordRight, resetAnchor: false);

    // Core Movement Logic
    private void Move(Func<TextPosition> computeNewPosition, bool resetAnchor)
    {
        _session.Caret = computeNewPosition();
        if (resetAnchor) _session.Anchor = _session.Caret;
    }

    // Position Computation Helpers
    private TextPosition ComputeLeft()
    {
        var caret = _session.Caret;

        if (caret.Column > 0) return new TextPosition(caret.Line, caret.Column - 1);
        if (caret.Line > 0) return new TextPosition(caret.Line - 1, GetLineLength(caret.Line - 1));
        return caret;
    }

    private TextPosition ComputeRight()
    {
        var caret = _session.Caret;

        if (caret.Column < CurrentLineLength) return new TextPosition(caret.Line, caret.Column + 1);
        if (caret.Line < LastLineIndex) return new TextPosition(caret.Line + 1, 0);
        return caret;
    }

    private TextPosition ComputeUp()
    {
        var caret = _session.Caret;
        if (caret.Line == 0) return new TextPosition(0, 0);

        int newCol = Math.Min(caret.Column, GetLineLength(caret.Line - 1));
        return new TextPosition(caret.Line - 1, newCol);
    }

    private TextPosition ComputeDown()
    {
        var caret = _session.Caret;
        if (caret.Line == LastLineIndex) return new TextPosition(caret.Line, CurrentLineLength);

        int newCol = Math.Min(caret.Column, GetLineLength(caret.Line + 1));
        return new TextPosition(caret.Line + 1, newCol);
    }

    private TextPosition ComputePageUp(int pageHeight)
    {
        var caret = _session.Caret;
        if (caret.Line == 0) return new TextPosition(0, 0);

        int targetLine = Math.Max(0, caret.Line - pageHeight);
        int newCol = Math.Min(caret.Column, GetLineLength(targetLine));
        return new TextPosition(targetLine, newCol);
    }

    private TextPosition ComputePageDown(int pageHeight)
    {
        var caret = _session.Caret;
        if (caret.Line == LastLineIndex) return new TextPosition(LastLineIndex, GetLineLength(LastLineIndex));

        int targetLine = Math.Min(LastLineIndex, caret.Line + pageHeight);
        int newCol = Math.Min(caret.Column, GetLineLength(targetLine));
        return new TextPosition(targetLine, newCol);
    }

    private TextPosition ComputeDocumentEnd() => new(LastLineIndex, GetLineLength(LastLineIndex));

    private TextPosition ComputeWordLeft()
    {
        var caret = _session.Caret;
        int line = caret.Line;
        int col = caret.Column;

        // At start of line, move to end of previous line
        if (col == 0)
        {
            if (line == 0) return caret;
            return new TextPosition(line - 1, GetLineLength(line - 1));
        }

        string content = _session.Document.Lines[line].Content;

        // Skip non-word characters backward
        while (col > 0 && !IsWordChar(content[col - 1]))
        {
            col--;
        }

        // Skip word characters backward
        while (col > 0 && IsWordChar(content[col - 1]))
        {
            col--;
        }

        return new TextPosition(line, col);
    }

    private TextPosition ComputeWordRight()
    {
        var caret = _session.Caret;
        int line = caret.Line;
        int col = caret.Column;
        string content = _session.Document.Lines[line].Content;

        // At end of line, move to start of next line
        if (col >= content.Length)
        {
            if (line >= LastLineIndex) return caret;
            return new TextPosition(line + 1, 0);
        }

        // Skip word characters forward
        while (col < content.Length && IsWordChar(content[col]))
        {
            col++;
        }

        // Skip non-word characters forward
        while (col < content.Length && !IsWordChar(content[col]))
        {
            col++;
        }

        return new TextPosition(line, col);
    }

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_';

    private int LastLineIndex => _session.Document.Lines.Count - 1;

    private int CurrentLineLength => GetLineLength(_session.Caret.Line);

    private int GetLineLength(int line) => _session.Document.Lines[line].Content.Length;
}
