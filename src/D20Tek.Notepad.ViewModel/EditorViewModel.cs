using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.ViewModel.Rendering;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private List<ViewLine> _visibleLines = [];
    private int _viewportWidth;

    // Events
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

    // Rendering
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
