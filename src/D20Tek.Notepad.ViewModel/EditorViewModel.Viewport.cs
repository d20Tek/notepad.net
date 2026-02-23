namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public int GetMaxLineLength() => Session.Document.Lines.Max(l => l.Content.Length);

    public int GetTotalVisualLineCount() => Settings.WordWrapEnabled
        ? WordWrapHelper.GetTotalVisualLineCount(Session.Document, _viewportWidth)
        : Session.Document.Lines.Count;

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
            // In word wrap mode, calculate the visual line index for the caret position
            int visualLineIndex = WordWrapHelper.GetVisualLineIndexForPosition(
                Session.Document, _viewportWidth, Session.Caret.Line, Session.Caret.Column);
            Viewport.EnsureLineVisible(visualLineIndex, GetTotalVisualLineCount());

            // No horizontal scrolling in word wrap mode
        }
        else
        {
            Viewport.EnsureLineVisible(Session.Caret.Line, Session.Document.Lines.Count);
            Viewport.EnsureColumnVisible(Session.Caret.Column, _viewportWidth);
        }

        Refresh();
    }
}
