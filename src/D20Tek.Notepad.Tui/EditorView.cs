using D20Tek.Notepad.Tui.Input;
using static Terminal.Gui.Application;

namespace D20Tek.Notepad.Tui;

public sealed class EditorView : View, IDisposable
{
    private readonly EditorViewModel _viewModel;
    private readonly TerminalGuiRenderer _renderer;
    private bool _disposed;

    public EditorView(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _renderer = new TerminalGuiRenderer(this);

        CanFocus = true;
        WantMousePositionReports = true;

        Resized += OnViewResized;
        
        // Subscribe to ViewModel events
        _viewModel.ViewChanged += OnViewChanged;
        _viewModel.CaretMoved += OnCaretMoved;
        _viewModel.SelectionChanged += OnSelectionChanged;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // Unsubscribe from ViewModel events to prevent memory leaks
        _viewModel.ViewChanged -= OnViewChanged;
        _viewModel.CaretMoved -= OnCaretMoved;
        _viewModel.SelectionChanged -= OnSelectionChanged;

        Resized -= OnViewResized;
    }

    private void OnViewChanged() => RedrawEditor();
    private void OnCaretMoved(ViewPosition _) => RedrawEditor();
    private void OnSelectionChanged(SelectionViewRange? _) => RedrawEditor();

    private void RedrawEditor()
    {
        // Terminal.Gui will call Redraw(), but we can force it when needed
        SetNeedsDisplay();
    }

    public override void Redraw(Rect bounds)
    {
        base.Redraw(bounds);

        // Update viewport dimensions based on current view size
        UpdateViewportSize();

        // Render the entire frame
        _viewModel.RenderFrame(_renderer);
    }

    public override void PositionCursor()
    {
        var caretLine = _viewModel.Session.Caret.Line;
        var caretCol = _viewModel.Session.Caret.Column;
        var verticalOffset = _viewModel.Viewport.FirstVisibleLine;
        var viewportHeight = _viewModel.Viewport.VisibleLineCount;

        bool caretVisible =
            caretLine >= verticalOffset &&
            caretLine < verticalOffset + viewportHeight &&
            caretCol >= _viewModel.Viewport.HorizontalOffset &&
            caretCol < _viewModel.Viewport.HorizontalOffset + _viewModel.ViewportWidth;

        if (HasFocus && caretVisible)
        {
            int screenX = caretCol - _viewModel.Viewport.HorizontalOffset + Frame.X;
            int screenY = caretLine - verticalOffset + Frame.Y;

            Application.Driver.SetCursorVisibility(CursorVisibility.Default);
            Application.Driver.Move(screenX, screenY);
        }
        else
        {
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
        }
    }

    private void UpdateViewportSize()
    {
        // Set viewport dimensions based on the view's bounds
        if (Bounds.Height != _viewModel.Viewport.VisibleLineCount)
        {
            _viewModel.SetViewportHeight(Bounds.Height);
        }

        // Optionally set width for horizontal scrolling
        if (Bounds.Width > 0)
        {
            _viewModel.SetViewportWidth(Bounds.Width);
        }
    }

    private void OnViewResized(ResizedEventArgs args)
    {
        _viewModel.SetViewportWidth(Frame.Width);
        _viewModel.SetViewportHeight(Frame.Height);
        _viewModel.EnsureCaretVisible();

        SetNeedsDisplay();
    }

    public override bool MouseEvent(MouseEvent me)
    {
        if (MouseBindings.TryExecute(_viewModel, me))
        {
            SetNeedsDisplay();
            return true;
        }

        return base.MouseEvent(me);
    }

    public override bool ProcessKey(KeyEvent keyEvent) =>
        KeyboardHandler.ProcessKey(_viewModel, keyEvent) || base.ProcessKey(keyEvent);
}
