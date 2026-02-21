using D20Tek.Notepad.Tui.Input;
using static Terminal.Gui.Application;

namespace D20Tek.Notepad.Tui;

public sealed class EditorView : View, IDisposable
{
    private readonly EditorViewModel _viewModel;
    private readonly TerminalGuiRenderer _renderer;
    private ScrollBarView? _vScrollBar;
    private ScrollBarView? _hScrollBar;
    private bool _scrollBarsInitialized;
    private bool _disposed;

    public EditorView(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _renderer = new TerminalGuiRenderer(this);

        CanFocus = true;
        WantMousePositionReports = true;
        ColorScheme = Colors.Dialog;

        Added += OnAddedToSuperView;
        Resized += OnViewResized;
        
        // Subscribe to ViewModel events
        _viewModel.ViewChanged += OnViewChanged;
        _viewModel.CaretMoved += OnCaretMoved;
        _viewModel.SelectionChanged += OnSelectionChanged;
    }

    public new void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // Unsubscribe from ScrollBar events
        if (_vScrollBar != null) _vScrollBar.ChangedPosition -= OnVerticalScrollChanged;
        if (_hScrollBar != null) _hScrollBar.ChangedPosition -= OnHorizontalScrollChanged;

        // Unsubscribe from ViewModel events to prevent memory leaks
        _viewModel.ViewChanged -= OnViewChanged;
        _viewModel.CaretMoved -= OnCaretMoved;
        _viewModel.SelectionChanged -= OnSelectionChanged;

        Resized -= OnViewResized;
        Added -= OnAddedToSuperView;
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
        // Update viewport dimensions based on current view size
        UpdateViewportSize();
        UpdateScrollBars();

        // Render editor content FIRST
        _viewModel.RenderFrame(_renderer);

        // Then let base.Redraw draw child views (scrollbars) ON TOP
        base.Redraw(bounds);
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

        UpdateScrollBars();
        SetNeedsDisplay();
    }

    public override bool MouseEvent(MouseEvent me)
    {
        // Check if click is in the scrollbar area - let children handle it
        bool inVerticalScrollbar = me.X >= Bounds.Width - 1;
        bool inHorizontalScrollbar = me.Y >= Bounds.Height - 1;
        
        if (inVerticalScrollbar || inHorizontalScrollbar)
        {
            // Don't handle - let it propagate to child scrollbars
            return base.MouseEvent(me);
        }

        if (MouseBindings.TryExecute(_viewModel, me))
        {
            SetNeedsDisplay();
            return true;
        }

        return base.MouseEvent(me);
    }

    public override bool ProcessKey(KeyEvent keyEvent) =>
        KeyBindings.TryExecute(_viewModel, keyEvent) || base.ProcessKey(keyEvent);

    private void OnAddedToSuperView(View superView)
    {
        if (_scrollBarsInitialized) return;
        _scrollBarsInitialized = true;

        _vScrollBar = new ScrollBarView(this, true)
        {
            Visible = true,
            AutoHideScrollBars = false,
            Size = 1
        };

        _hScrollBar = new ScrollBarView(this, false)
        {
            Visible = true,
            AutoHideScrollBars = false,
            Size = 1
        };

        // Link them (v1.19 uses this for some internal logic)
        _vScrollBar.OtherScrollBarView = _hScrollBar;
        _hScrollBar.OtherScrollBarView = _vScrollBar;

        // Subscribe to scroll position changes
        _vScrollBar.ChangedPosition += OnVerticalScrollChanged;
        _hScrollBar.ChangedPosition += OnHorizontalScrollChanged;

        // Add as children of THIS view (not superview)
        Add(_vScrollBar);
        Add(_hScrollBar);

        UpdateScrollBarFrames();
        UpdateScrollBars();
    }

    private void OnVerticalScrollChanged()
    {
        if (_vScrollBar == null) return;
        
        int newFirstLine = _vScrollBar.Position;
        int delta = newFirstLine - _viewModel.Viewport.FirstVisibleLine;
        if (delta != 0)
        {
            _viewModel.ScrollLines(delta);
        }
    }

    private void OnHorizontalScrollChanged()
    {
        if (_hScrollBar == null) return;
        
        int newOffset = _hScrollBar.Position;
        int delta = newOffset - _viewModel.Viewport.HorizontalOffset;
        if (delta != 0)
        {
            _viewModel.ScrollColumns(delta);
        }
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        UpdateScrollBarFrames();
    }

    private void UpdateScrollBarFrames()
    {
        if (_vScrollBar == null || _hScrollBar == null) return;
        
        // Guard against zero-sized bounds during early initialization
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        // Use Bounds-relative coordinates since scrollbars are children
        // Set X/Y explicitly to override any host-based positioning from ScrollBarView
        _vScrollBar.X = Bounds.Width - 1;
        _vScrollBar.Y = 0;
        _vScrollBar.Width = 1;
        _vScrollBar.Height = Bounds.Height - 1;

        _hScrollBar.X = 0;
        _hScrollBar.Y = Bounds.Height - 1;
        _hScrollBar.Width = Bounds.Width - 1;
        _hScrollBar.Height = 1;
    }

    private void UpdateScrollBars()
    {
        // Guard against updates before layout is complete
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        if (_vScrollBar != null)
        {
            var lineCount = _viewModel.Session.Document.LineCount;
            var viewportLines = _viewModel.Viewport.VisibleLineCount;

            // Ensure Size is at least 1 to prevent divide-by-zero in ScrollBarView
            _vScrollBar.Size = Math.Max(1, lineCount);
            _vScrollBar.Position = Math.Clamp(_viewModel.Viewport.FirstVisibleLine, 0, Math.Max(0, lineCount - 1));
            _vScrollBar.Visible = lineCount > viewportLines;
            _vScrollBar.SetNeedsDisplay();
        }

        if (_hScrollBar != null)
        {
            var maxLineLength = _viewModel.GetMaxLineLength();
            var viewportCols = _viewModel.ViewportWidth;

            // Ensure Size is at least 1 to prevent divide-by-zero in ScrollBarView
            _hScrollBar.Size = Math.Max(1, maxLineLength);
            _hScrollBar.Position = Math.Clamp(_viewModel.Viewport.HorizontalOffset, 0, Math.Max(0, maxLineLength - 1));
            _hScrollBar.Visible = maxLineLength > viewportCols;
            _hScrollBar.SetNeedsDisplay();
        }
    }
}
