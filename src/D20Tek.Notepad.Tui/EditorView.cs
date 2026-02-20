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
