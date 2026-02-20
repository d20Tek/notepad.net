using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public SelectionSegment? GetSelectionSegmentForLine(int viewLineIndex)
    {
        if (SelectionViewRange is not { } sel) return null;
        if (viewLineIndex < sel.Start.LineIndex || viewLineIndex > sel.End.LineIndex) return null;

        var textLength = VisibleLines[viewLineIndex].Text.Length;
        var (startLine, endLine) = (sel.Start.LineIndex, sel.End.LineIndex);
        var (startCol, endCol) = (Math.Max(0, sel.Start.Column), Math.Max(0, sel.End.Column));

        return (viewLineIndex == startLine, viewLineIndex == endLine) switch
        {
            (true, true)  => new SelectionSegment(startCol, Math.Max(startCol, endCol)),   // single line
            (true, false) => new SelectionSegment(startCol, textLength),                   // first line
            (false, true) => new SelectionSegment(0, endCol),                              // last line
            _             => new SelectionSegment(0, textLength)                           // middle line
        };
    }

    public void ExtendSelectionTo(int line, int column)
    {
        // Clamp line
        line = Math.Max(0, Math.Min(line, Session.Document.Lines.Count - 1));

        // Clamp column
        int lineLength = Session.Document.Lines[line].Content.Length;
        column = Math.Max(0, Math.Min(column, lineLength));

        Session.Caret = new TextPosition(line, column);

        // IMPORTANT: Do NOT modify session.Anchor here. Anchor was set when selection began.
    }

    private static bool SelectionChangedNeeded(SelectionViewRange? oldSel, SelectionViewRange? newSel)
    {
        if (oldSel is null && newSel is null) return false;
        if (oldSel is null || newSel is null) return true;

        return !oldSel.Value.Equals(newSel.Value);
    }
}
