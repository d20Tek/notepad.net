namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public void SetViewportHeight(int visibleLineCount) => SetWithRefresh(() =>
        Viewport.SetVisibleLineCount(visibleLineCount));

    public void SetViewportWidth(int width) => SetWithRefresh(() => _viewportWidth = Math.Max(0, width));

    public void ScrollLines(int delta) => SetWithRefresh(() =>
        Viewport.ScrollLines(delta, Session.Document.Lines.Count));

    public void ScrollPages(int deltaPages) => SetWithRefresh(() =>
        Viewport.ScrollPages(deltaPages, Session.Document.Lines.Count));

    public void ScrollColumns(int delta) => SetWithRefresh(() => Viewport.ScrollColumns(delta));

    public void EnsureCaretVisible()
    {
        Viewport.EnsureLineVisible(Session.Caret.Line, Session.Document.Lines.Count);
        Viewport.EnsureColumnVisible(Session.Caret.Column, _viewportWidth);
        Refresh();
    }
}
