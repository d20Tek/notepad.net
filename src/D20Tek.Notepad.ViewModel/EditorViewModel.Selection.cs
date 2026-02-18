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
}
