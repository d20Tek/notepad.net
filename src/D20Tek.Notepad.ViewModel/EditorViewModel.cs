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
        : this(session, commandService, EditorSettings.Default)
    {
    }

    public EditorViewModel(EditorSession session, EditorCommandService commandService, EditorSettings settings)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(commandService);
        ArgumentNullException.ThrowIfNull(settings);
        Session = session;
        Commands = commandService;
        Settings = settings;
        Viewport = new Viewport();
    }

    public EditorSession Session { get; }

    public EditorCommandService Commands { get; }

    public EditorSettings Settings { get; }

    public Viewport Viewport { get; }

    public int ViewportWidth => _viewportWidth;

    public IReadOnlyList<ViewLine> VisibleLines => _visibleLines;

    public ViewPosition CaretViewPosition { get; private set; }

    public SelectionViewRange? SelectionViewRange { get; set; }

    // ============================================================
    // Viewport Management
    // ============================================================

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

    // ============================================================
    // Navigation (delegates to CaretNavigator with viewport awareness)
    // ============================================================

    public void MoveLeft() { Session.Navigator.MoveLeft(); EnsureCaretVisible(); }
    public void MoveRight() { Session.Navigator.MoveRight(); EnsureCaretVisible(); }
    public void MoveUp() { Session.Navigator.MoveUp(); EnsureCaretVisible(); }
    public void MoveDown() { Session.Navigator.MoveDown(); EnsureCaretVisible(); }
    public void MovePageUp() { Session.Navigator.MovePageUp(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    public void MovePageDown() { Session.Navigator.MovePageDown(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    public void MoveToLineStart() { Session.Navigator.MoveToLineStart(); EnsureCaretVisible(); }
    public void MoveToLineEnd() { Session.Navigator.MoveToLineEnd(); EnsureCaretVisible(); }
    public void MoveToDocumentStart() { Session.Navigator.MoveToDocumentStart(); EnsureCaretVisible(); }
    public void MoveToDocumentEnd() { Session.Navigator.MoveToDocumentEnd(); EnsureCaretVisible(); }

    // ============================================================
    // Selection Extension (delegates to CaretNavigator with viewport awareness)
    // ============================================================

    public void ExtendLeft() { Session.Navigator.ExtendLeft(); EnsureCaretVisible(); }
    public void ExtendRight() { Session.Navigator.ExtendRight(); EnsureCaretVisible(); }
    public void ExtendUp() { Session.Navigator.ExtendUp(); EnsureCaretVisible(); }
    public void ExtendDown() { Session.Navigator.ExtendDown(); EnsureCaretVisible(); }
    public void ExtendPageUp() { Session.Navigator.ExtendPageUp(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    public void ExtendPageDown() { Session.Navigator.ExtendPageDown(Viewport.VisibleLineCount); EnsureCaretVisible(); }
    public void ExtendToLineStart() { Session.Navigator.ExtendToLineStart(); EnsureCaretVisible(); }
    public void ExtendToLineEnd() { Session.Navigator.ExtendToLineEnd(); EnsureCaretVisible(); }
    public void ExtendToDocumentStart() { Session.Navigator.ExtendToDocumentStart(); EnsureCaretVisible(); }
    public void ExtendToDocumentEnd() { Session.Navigator.ExtendToDocumentEnd(); EnsureCaretVisible(); }

    // ============================================================
    // Caret/Selection Helpers
    // ============================================================

    public int GetLineLength(int lineIndex) => Session.GetLineLength(lineIndex);

    public void SetAnchorToCaret() => Session.Anchor = Session.Caret;

    public void MoveCaretTo(int line, int column)
    {
        var newPos = Session.ClampToDocument(line, column);
        Session.Caret = newPos;
        Session.Anchor = newPos;
    }

    // ============================================================
    // Rendering
    // ============================================================

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
