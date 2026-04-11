namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public int GetMaxLineLength() => Session.Document.MaxLineLength;

    public int GetTotalVisualLineCount()
    {
        if (!Settings.WordWrapEnabled) return Session.Document.Lines.Count;
        SyncWrapIndex();
        return _wrapIndex.TotalVisualLineCount;
    }

    public void SetViewportHeight(int visibleLineCount) => SetWithRefresh(() =>
        Viewport.SetVisibleLineCount(visibleLineCount));

    public void SetViewportWidth(int width) => SetWithRefresh(() => _viewportWidth = Math.Max(0, width));

    public void ScrollLines(int delta) => SetWithRefresh(() =>
        Viewport.ScrollLines(delta, GetTotalVisualLineCount()));

    public void ScrollPages(int deltaPages) => SetWithRefresh(() =>
        Viewport.ScrollPages(deltaPages, GetTotalVisualLineCount()));

    public void ScrollColumns(int delta)
    {
        // Disable horizontal scrolling in word wrap mode
        if (Settings.WordWrapEnabled) return;

        SetWithRefresh(() => Viewport.ScrollColumns(delta));
    }

    public void EnsureCaretVisible()
    {
        if (Settings.WordWrapEnabled)
        {
            SyncWrapIndex();
            int visualLineIndex = _wrapIndex.GetVisualLineIndexForDocumentLine(Session.Caret.Line);

            var lineText = Session.Document.Lines[Session.Caret.Line].Content;
            var segments = WordWrapCalculator.WrapLine(lineText, _viewportWidth);
            for (int i = 0; i < segments.Count - 1; i++)
            {
                int segmentEnd = segments[i].StartColumn + segments[i].Length;
                if (Session.Caret.Column <= segmentEnd)
                {
                    visualLineIndex += i;
                    break;
                }

                if (i == segments.Count - 2)
                    visualLineIndex += segments.Count - 1;
            }

            Viewport.EnsureLineVisible(visualLineIndex, _wrapIndex.TotalVisualLineCount);
        }
        else
        {
            Viewport.EnsureLineVisible(Session.Caret.Line, Session.Document.Lines.Count);
            Viewport.EnsureColumnVisible(Session.Caret.Column, _viewportWidth);
        }

        Refresh();
    }
}
