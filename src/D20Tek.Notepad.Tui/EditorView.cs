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

        _viewModel.ViewChanged += OnViewModelChanged;
        _viewModel.CaretMoved += OnViewModelChanged;
        _viewModel.SelectionChanged += OnViewModelChanged;
        _viewModel.WordWrapChanged += OnWordWrapChanged;
    }

    public new void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        UnsubscribeScrollBarEvents();

        _viewModel.ViewChanged -= OnViewModelChanged;
        _viewModel.CaretMoved -= OnViewModelChanged;
        _viewModel.SelectionChanged -= OnViewModelChanged;
        _viewModel.WordWrapChanged -= OnWordWrapChanged;

        Resized -= OnViewResized;
        Added -= OnAddedToSuperView;
    }

    public override void Redraw(Rect bounds)
    {
        UpdateViewportSize();
        SyncScrollBarState();

        _viewModel.RenderFrame(_renderer);
        base.Redraw(bounds);
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        UpdateScrollBarLayout();
    }

    public override void PositionCursor()
    {
        if (HasFocus && IsCaretVisible())
        {
            var (screenX, screenY) = GetCaretScreenPosition();
            Application.Driver.SetCursorVisibility(CursorVisibility.Default);
            Application.Driver.Move(screenX, screenY);
        }
        else
        {
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
        }
    }

    public override bool MouseEvent(MouseEvent me)
    {
        if (IsInScrollBarArea(me.X, me.Y))
        {
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

    private void OnViewModelChanged() => SetNeedsDisplay();
    private void OnViewModelChanged(ViewPosition _) => SetNeedsDisplay();
    private void OnViewModelChanged(SelectionViewRange? _) => SetNeedsDisplay();

    private void OnWordWrapChanged(bool _)
    {
        SyncScrollBarState();
        SetNeedsDisplay();
    }

    private void OnViewResized(ResizedEventArgs args)
    {
        // Initial resize - scrollbars may not exist yet, so use full dimensions
        // UpdateViewportSize in Redraw will adjust once scrollbar visibility is known
        _viewModel.SetViewportWidth(Frame.Width);
        _viewModel.SetViewportHeight(Frame.Height);
        _viewModel.EnsureCaretVisible();

        SyncScrollBarState();
        SetNeedsDisplay();
    }

    private void OnAddedToSuperView(View superView)
    {
        if (_scrollBarsInitialized) return;
        _scrollBarsInitialized = true;

        InitializeScrollBars();
        UpdateScrollBarLayout();
        SyncScrollBarState();
    }

    private void OnVerticalScrollChanged()
    {
        if (_vScrollBar is null) return;

        int delta = _vScrollBar.Position - _viewModel.Viewport.FirstVisibleLine;
        if (delta != 0)
        {
            _viewModel.ScrollLines(delta);
        }
    }

    private void OnHorizontalScrollChanged()
    {
        if (_hScrollBar is null) return;

        int delta = _hScrollBar.Position - _viewModel.Viewport.HorizontalOffset;
        if (delta != 0)
        {
            _viewModel.ScrollColumns(delta);
        }
    }

    private void UpdateViewportSize()
    {
        // Account for scrollbar space when calculating effective viewport dimensions
        int effectiveHeight = Bounds.Height;
        int effectiveWidth = Bounds.Width;

        // Reserve space for horizontal scrollbar if it will be visible
        if (_hScrollBar?.Visible == true)
        {
            effectiveHeight = Math.Max(0, effectiveHeight - 1);
        }

        // Reserve space for vertical scrollbar if it will be visible
        if (_vScrollBar?.Visible == true)
        {
            effectiveWidth = Math.Max(0, effectiveWidth - 1);
        }

        if (effectiveHeight != _viewModel.Viewport.VisibleLineCount)
        {
            _viewModel.SetViewportHeight(effectiveHeight);
        }

        if (effectiveWidth > 0)
        {
            _viewModel.SetViewportWidth(effectiveWidth);
        }
    }

    private bool IsCaretVisible()
    {
        var caretViewPos = _viewModel.CaretViewPosition;
        var viewport = _viewModel.Viewport;

        // CaretViewPosition is already in view coordinates (relative to visible lines)
        return caretViewPos.LineIndex >= 0 &&
               caretViewPos.LineIndex < viewport.VisibleLineCount &&
               caretViewPos.Column >= 0 &&
               caretViewPos.Column < _viewModel.ViewportWidth;
    }

    private (int X, int Y) GetCaretScreenPosition()
    {
        var caretViewPos = _viewModel.CaretViewPosition;

        // CaretViewPosition is already relative to visible area
        int screenX = caretViewPos.Column + Frame.X;
        int screenY = caretViewPos.LineIndex + Frame.Y;

        return (screenX, screenY);
    }

    private void InitializeScrollBars()
    {
        _vScrollBar = CreateScrollBar(isVertical: true);
        _hScrollBar = CreateScrollBar(isVertical: false);

        _vScrollBar.OtherScrollBarView = _hScrollBar;
        _hScrollBar.OtherScrollBarView = _vScrollBar;

        _vScrollBar.ChangedPosition += OnVerticalScrollChanged;
        _hScrollBar.ChangedPosition += OnHorizontalScrollChanged;

        Add(_vScrollBar);
        Add(_hScrollBar);
    }

    private ScrollBarView CreateScrollBar(bool isVertical) => new(this, isVertical)
    {
        Visible = true,
        AutoHideScrollBars = false,
        Size = 1
    };

    private void UnsubscribeScrollBarEvents()
    {
        if (_vScrollBar is not null)
            _vScrollBar.ChangedPosition -= OnVerticalScrollChanged;

        if (_hScrollBar is not null)
            _hScrollBar.ChangedPosition -= OnHorizontalScrollChanged;
    }

    private void UpdateScrollBarLayout()
    {
        if (_vScrollBar is null || _hScrollBar is null) return;
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        _vScrollBar.X = Bounds.Width - 1;
        _vScrollBar.Y = 0;
        _vScrollBar.Width = 1;
        _vScrollBar.Height = Bounds.Height;

        _hScrollBar.X = 0;
        _hScrollBar.Y = Bounds.Height - 1;
        _hScrollBar.Width = Bounds.Width - 1;
        _hScrollBar.Height = 1;
    }

    private void SyncScrollBarState()
    {
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        SyncVerticalScrollBar();
        SyncHorizontalScrollBar();
    }

    private void SyncVerticalScrollBar()
    {
        if (_vScrollBar is null) return;

        // In word wrap mode, use visual line count; otherwise use document line count
        var totalLines = _viewModel.GetTotalVisualLineCount();
        var viewportLines = _viewModel.Viewport.VisibleLineCount;
        int maxPosition = Math.Max(0, totalLines - viewportLines);

        _vScrollBar.Size = Math.Max(1, totalLines);
        _vScrollBar.Position = Math.Clamp(_viewModel.Viewport.FirstVisibleLine, 0, maxPosition);
        _vScrollBar.Visible = totalLines > viewportLines;
        _vScrollBar.SetNeedsDisplay();
    }

    private void SyncHorizontalScrollBar()
    {
        if (_hScrollBar is null) return;

        // Hide horizontal scrollbar when word wrap is enabled
        if (_viewModel.IsWordWrapEnabled)
        {
            _hScrollBar.Visible = false;
            _hScrollBar.SetNeedsDisplay();
            return;
        }

        var maxLineLength = _viewModel.GetMaxLineLength();
        var viewportCols = _viewModel.ViewportWidth;
        int maxPosition = Math.Max(0, maxLineLength - viewportCols);

        _hScrollBar.Size = Math.Max(1, maxLineLength + 1);
        _hScrollBar.Position = Math.Clamp(_viewModel.Viewport.HorizontalOffset, 0, maxPosition);
        _hScrollBar.Visible = maxLineLength > viewportCols;
        _hScrollBar.SetNeedsDisplay();
    }

    private bool IsInScrollBarArea(int x, int y) =>
        x >= Bounds.Width - 1 || y >= Bounds.Height - 1;
}
