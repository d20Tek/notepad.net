using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.ViewModel.Rendering;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel(EditorSession session, EditorCommandService commandService)
{
    private List<ViewLine> _visibleLines = [];
    private int _viewportWidth;

    public event Action? ViewChanged;
    public event Action<ViewPosition>? CaretMoved;
    public event Action<SelectionViewRange?>? SelectionChanged;

    public EditorSession Session { get; } = session ?? throw new ArgumentNullException(nameof(session));

    public EditorCommandService Commands { get; } = commandService ?? throw new ArgumentNullException(nameof(session));

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

    public void EnsureCaretVisible()
    {
        Viewport.EnsureLineVisible(Session.Caret.Line, Session.Document.Lines.Count);
        Viewport.EnsureColumnVisible(Session.Caret.Column, _viewportWidth);
        Refresh();
    }

    public void ScrollColumns(int delta) => SetWithRefresh(() => Viewport.ScrollColumns(delta));

    public void SetViewportWidth(int width) => SetWithRefresh(() => _viewportWidth = Math.Max(0, width));

    public void RenderFrame(IEditorRenderer renderer)
    {
        renderer.BeginFrame(this);

        for (int i = 0; i < VisibleLines.Count; i++)
        {
            var line = VisibleLines[i];
            var segment = GetSelectionSegmentForLine(i);

            renderer.RenderLine(i, line, segment);
        }

        renderer.RenderCaret(CaretViewPosition);

        renderer.EndFrame();
    }

    public void Refresh()
    {
        var oldCaret = CaretViewPosition;
        var oldSelection = SelectionViewRange;

        _visibleLines = BuildVisibleLines(Viewport.FirstVisibleLine, Viewport.VisibleLineCount, Session.Document);

        CaretViewPosition = MapCaret();
        SelectionViewRange = MapSelection();

        // fire events
        ViewChanged?.Invoke();
        if (!CaretViewPosition.Equals(oldCaret)) CaretMoved?.Invoke(CaretViewPosition);
        if (SelectionChangedNeeded(oldSelection, SelectionViewRange)) SelectionChanged?.Invoke(SelectionViewRange);
    }

    private void SetWithRefresh(Action setAction)
    {
        setAction();
        Refresh();
    }

    private List<ViewLine> BuildVisibleLines(int first, int count, IDocument doc)
    {
        var visibleLines = new List<ViewLine>(count);
        var total = doc.Lines.Count;
        int lastExclusive = Math.Min(first + count, total);     // last visible line index

        int hOffset = Viewport.HorizontalOffset;
        int width = _viewportWidth;

        for (int docLine = first; docLine < lastExclusive; docLine++)
        {
            var fullText = doc.Lines[docLine].Content;

            string sliced;
            if (width <= 0)
            {
                // no horizontal limit - return full text (minus offset)
                sliced = hOffset < fullText.Length ? fullText.Substring(hOffset) : string.Empty;
            }
            else if (hOffset < fullText.Length)
            {
                sliced = fullText.Substring(hOffset, Math.Min(width, fullText.Length - hOffset));
            }
            else
            {
                sliced = string.Empty;
            }

            visibleLines.Add(new ViewLine(docLine, sliced));
        }

        return visibleLines;
    }

    private ViewPosition MapCaret() => new(
        Math.Max(0, Session.Caret.Line - Viewport.FirstVisibleLine),
        Math.Max(0, Session.Caret.Column - Viewport.HorizontalOffset));

    private SelectionViewRange? MapSelection()
    {
        if (!Session.HasSelection) return null;

        var raw = ViewMapping.DocumentSelectionToView(
            Session.Anchor, Session.Caret, Viewport.FirstVisibleLine, Viewport.VisibleLineCount);

        return new SelectionViewRange(
            new ViewPosition(raw.Start.LineIndex, Math.Max(0, raw.Start.Column - Viewport.HorizontalOffset)),
            new ViewPosition(raw.End.LineIndex, Math.Max(0, raw.End.Column - Viewport.HorizontalOffset))).Normalize();
    }
}
