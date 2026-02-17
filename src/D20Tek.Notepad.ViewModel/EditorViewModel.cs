using D20Tek.Notepad.Core.Editing;

namespace D20Tek.Notepad.ViewModel;

public sealed class EditorViewModel(EditorSession session)
{
    private List<ViewLine> _visibleLines = [];

    public EditorSession Session { get; } = session ?? throw new ArgumentNullException(nameof(session));

    public Viewport Viewport { get; } = new Viewport();

    public IReadOnlyList<ViewLine> VisibleLines => _visibleLines;

    public ViewPosition CaretViewPosition { get; private set; }

    public SelectionViewRange? SelectionViewRange { get; private set; }

    public void SetViewportHeight(int visibleLineCount) => SetWithRefresh(() =>
        Viewport.SetVisibleLineCount(visibleLineCount));

    public void ScrollLines(int delta) => SetWithRefresh(() =>
        Viewport.ScrollLines(delta, Session.Document.Lines.Count));

    public void ScrollPages(int deltaPages) => SetWithRefresh(() =>
        Viewport.ScrollPages(deltaPages, Session.Document.Lines.Count));

    public void EnsureCaretVisible() => SetWithRefresh(() => 
        Viewport.EnsureLineVisible(Session.Caret.Line, Session.Document.Lines.Count));

    public void Refresh()
    {
        _visibleLines = BuildVisibleLines(
            Viewport.FirstVisibleLine, Viewport.VisibleLineCount, Session.Document.Lines.Count);

        CaretViewPosition = MapCaret();
        SelectionViewRange = MapSelection();
    }

    private void SetWithRefresh(Action setAction)
    {
        setAction();
        Refresh();
    }

    private List<ViewLine> BuildVisibleLines(int first, int count, int total)
    {
        var visibleLines = new List<ViewLine>();

        for (int i = 0; i < count; i++)
        {
            int docLine = first + i;
            if (docLine >= total) break;

            var text = Session.Document.Lines[docLine].Content;
            visibleLines.Add(new ViewLine(docLine, text));
        }

        return visibleLines;
    }

    private ViewPosition MapCaret() => ViewMapping.ClampToViewport(
        Session.Caret, Viewport.FirstVisibleLine, Viewport.VisibleLineCount);

    private SelectionViewRange? MapSelection() =>
        Session.HasSelection
            ? ViewMapping.DocumentSelectionToView(
                Session.Anchor,
                Session.Caret,
                Viewport.FirstVisibleLine,
                Viewport.VisibleLineCount)
            : null;
}
