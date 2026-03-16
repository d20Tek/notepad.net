using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.ViewModel.Rendering;
using System.Text;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private List<ViewLine> _visibleLines = [];
    private int _viewportWidth;
    private EditorSettings _settings;
    private readonly IClipboardService _clipboardService;
    private StatusDetails _currentStatus = StatusDetails.Empty;

    // Events
    public event Action? ViewChanged;
    public event Action<ViewPosition>? CaretMoved;
    public event Action<SelectionViewRange?>? SelectionChanged;
    public event Action<bool>? WordWrapChanged;
    public event Action<bool>? StatusBarChanged;
    public event Action<StatusDetails>? StatusChanged;

    public EditorViewModel(
        EditorSession session,
        EditorCommandService commandService,
        EditorSettings settings,
        IClipboardService clipboardService)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(commandService);
        ArgumentNullException.ThrowIfNull(settings);

        Session = session;
        Commands = commandService;
        _settings = settings;
        _clipboardService = clipboardService;
        Viewport = new Viewport();
        HookFilePathChanged();
    }

    public EditorSession Session { get; }

    public EditorCommandService Commands { get; }

    public EditorSettings Settings => _settings;

    public bool IsWordWrapEnabled => _settings.WordWrapEnabled;

    public bool IsStatusBarEnabled => _settings.StatusBarEnabled;

    public Viewport Viewport { get; }

    public int ViewportWidth => _viewportWidth;

    public IReadOnlyList<ViewLine> VisibleLines => _visibleLines;

    public ViewPosition CaretViewPosition { get; private set; }

    public SelectionViewRange? SelectionViewRange { get; set; }

    public StatusDetails CurrentStatus => _currentStatus;

    // Word Wrap Toggle
    public void ToggleWordWrap()
    {
        _settings = _settings with { WordWrapEnabled = !_settings.WordWrapEnabled };

        // Reset horizontal scroll when enabling word wrap
        if (_settings.WordWrapEnabled)
        {
            Viewport.SetHorizontalOffset(0);
        }

        Refresh();
        WordWrapChanged?.Invoke(_settings.WordWrapEnabled);
    }

    public void SetWordWrap(bool enabled)
    {
        if (_settings.WordWrapEnabled == enabled) return;

        _settings = _settings with { WordWrapEnabled = enabled };

        if (enabled)
        {
            Viewport.SetHorizontalOffset(0);
        }

        Refresh();
        WordWrapChanged?.Invoke(enabled);
    }

    public void ToggleStatusBar()
    {
        _settings = _settings with { StatusBarEnabled = !_settings.StatusBarEnabled };
        StatusBarChanged?.Invoke(_settings.StatusBarEnabled);
    }

    public void ChangeEncoding(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        Session.ChangeEncoding(encoding);
        CheckDirtyStateChanged();
        Refresh();
    }

    public bool IsCurrentEncoding(Encoding encoding) =>
        Session.Document.Encoding.WebName == encoding.WebName &&
        Session.Document.Encoding.GetPreamble().SequenceEqual(encoding.GetPreamble());

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

        var newStatus = BuildStatus();
        if (newStatus != _currentStatus)
        {
            _currentStatus = newStatus;
            StatusChanged?.Invoke(_currentStatus);
        }
    }

    private void SetWithRefresh(Action setAction)
    {
        setAction();
        Refresh();
    }
}
