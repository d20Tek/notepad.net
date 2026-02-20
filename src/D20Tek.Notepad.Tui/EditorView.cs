using static Terminal.Gui.Application;

namespace D20Tek.Notepad.Tui;

public sealed class EditorView : View
{
    private readonly EditorViewModel _viewModel;
    private readonly TerminalGuiRenderer _renderer;

    public EditorView(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _renderer = new TerminalGuiRenderer(this);

        CanFocus = true;
        WantMousePositionReports = true;

        Resized += OnViewResized;
        // Subscribe to ViewModel events
        _viewModel.ViewChanged += () => RedrawEditor();
        _viewModel.CaretMoved += _ => RedrawEditor();
        _viewModel.SelectionChanged += _ => RedrawEditor();
    }

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
        // todo: can this be moved to the TerminalGuiRenderer?
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

            Application.Driver.SetCursorVisibility(CursorVisibility.Box);
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
        if (MouseHandler.ProcessMouse(_viewModel, me))
        {
            SetNeedsDisplay();
            return true;
        }

        return base.MouseEvent(me);
    }

    public override bool ProcessKey(KeyEvent keyEvent) =>
        KeyboardHandler.ProcessKey(_viewModel, keyEvent) || base.ProcessKey(keyEvent);
}
