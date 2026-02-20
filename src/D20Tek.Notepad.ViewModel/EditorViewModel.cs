using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;
using D20Tek.Notepad.ViewModel.Rendering;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private List<ViewLine> _visibleLines = [];
    private int _viewportWidth;

    public event Action? ViewChanged;
    public event Action<ViewPosition>? CaretMoved;
    public event Action<SelectionViewRange?>? SelectionChanged;

    public EditorViewModel(EditorSession session, EditorCommandService commandService)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(commandService);
        Session = session;
        Commands = commandService;
        Navigator = new Navigator(this);
        Viewport = new Viewport();
    }

    public EditorSession Session { get; }

    public EditorCommandService Commands { get; }

    public Navigator Navigator { get; }

    public Viewport Viewport { get; }

    public int ViewportWidth { get { return _viewportWidth; } }

    public IReadOnlyList<ViewLine> VisibleLines => _visibleLines;

    public ViewPosition CaretViewPosition { get; private set; }

    public SelectionViewRange? SelectionViewRange { get; set; }

    public void SetViewportHeight(int visibleLineCount) => SetWithRefresh(() =>
        Viewport.SetVisibleLineCount(visibleLineCount));

    public void SetViewportWidth(int width) => SetWithRefresh(() => _viewportWidth = Math.Max(0, width));

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

    public int GetLineLength(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= Session.Document.Lines.Count)
            return 0;

        return Session.Document.Lines[lineIndex].Content.Length;
    }

    public void SetAnchorToCaret() => Session.Anchor = Session.Caret;

    public void MoveCaretTo(int line, int column)
    {
        // Clamp line
        line = Math.Max(0, Math.Min(line, Session.Document.Lines.Count - 1));

        // Clamp column
        int lineLength = Session.Document.Lines[line].Content.Length;
        column = Math.Max(0, Math.Min(column, lineLength));

        var newPos = new TextPosition(line, column);

        Session.Caret = newPos;
        Session.Anchor = newPos;
    }

    public void RenderFrame(IEditorRenderer renderer) => RendererLoop.RenderFull(this, renderer);
    
    public void Refresh()
    {
        var oldCaret = CaretViewPosition;
        var oldSelection = SelectionViewRange;

        _visibleLines = VisibleLinesBuilder.Build(this, _viewportWidth);

        CaretViewPosition = VisibleLinesBuilder.MapCaret(this);
        SelectionViewRange = VisibleLinesBuilder.MapSelection(this);

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
}
