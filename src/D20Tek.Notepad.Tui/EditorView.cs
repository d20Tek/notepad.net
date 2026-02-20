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

    public override bool ProcessKey(KeyEvent keyEvent)
    {
        bool shift = (keyEvent.Key & Key.ShiftMask) != 0;
        Key key = keyEvent.Key & ~Key.ShiftMask;   // strip modifiers

        switch (key)
        {
            // -----------------------------
            // Arrow Keys
            // -----------------------------
            case Key.CursorLeft:
                if (shift) _viewModel.Navigator.ExtendSelectionLeft();
                else _viewModel.Navigator.MoveCaretLeft();
                return true;

            case Key.CursorRight:
                if (shift) _viewModel.Navigator.ExtendSelectionRight();
                else _viewModel.Navigator.MoveCaretRight();
                return true;

            case Key.CursorUp:
                if (shift) _viewModel.Navigator.ExtendSelectionUp();
                else _viewModel.Navigator.MoveCaretUp();
                return true;

            case Key.CursorDown:
                if (shift) _viewModel.Navigator.ExtendSelectionDown();
                else _viewModel.Navigator.MoveCaretDown();
                return true;

            // -----------------------------
            // Home / End
            // -----------------------------
            case Key.Home:
                if (shift) _viewModel.Navigator.ExtendSelectionToLineStart();
                else _viewModel.Navigator.MoveToLineStart();
                return true;

            case Key.End:
                if (shift) _viewModel.Navigator.ExtendSelectionToLineEnd();
                else _viewModel.Navigator.MoveToLineEnd();
                return true;

            // -----------------------------
            // todo: Page Up / Page Down
            // -----------------------------
            //case Key.PageUp:
            //    if (shift) _viewModel.Navigator.ExtendSelectionPageUp();
            //    else _viewModel.Navigator.PageUp();
            //    return true;

            //case Key.PageDown:
            //    if (shift) _viewModel.Navigator.ExtendSelectionPageDown();
            //    else _viewModel.Navigator.PageDown();
            //    return true;
        }

        return base.ProcessKey(keyEvent);
    }
}
