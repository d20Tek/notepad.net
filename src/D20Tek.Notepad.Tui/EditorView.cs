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
    }

    public new void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        UnsubscribeScrollBarEvents();

        _viewModel.ViewChanged -= OnViewModelChanged;
        _viewModel.CaretMoved -= OnViewModelChanged;
        _viewModel.SelectionChanged -= OnViewModelChanged;

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
        var caret = _viewModel.Session.Caret;
        var viewport = _viewModel.Viewport;

        return caret.Line >= viewport.FirstVisibleLine &&
               caret.Line < viewport.FirstVisibleLine + viewport.VisibleLineCount &&
               caret.Column >= viewport.HorizontalOffset &&
               caret.Column < viewport.HorizontalOffset + _viewModel.ViewportWidth;
    }

    private (int X, int Y) GetCaretScreenPosition()
    {
        var caret = _viewModel.Session.Caret;
        var viewport = _viewModel.Viewport;

        int screenX = caret.Column - viewport.HorizontalOffset + Frame.X;
        int screenY = caret.Line - viewport.FirstVisibleLine + Frame.Y;

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
        _vScrollBar.Height = Bounds.Height - 1;

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

        var lineCount = _viewModel.Session.Document.LineCount;
        var viewportLines = _viewModel.Viewport.VisibleLineCount;
        int maxPosition = Math.Max(0, lineCount - viewportLines);

        _vScrollBar.Size = Math.Max(1, lineCount);
        _vScrollBar.Position = Math.Clamp(_viewModel.Viewport.FirstVisibleLine, 0, maxPosition);
        _vScrollBar.Visible = lineCount > viewportLines;
        _vScrollBar.SetNeedsDisplay();
    }

    private void SyncHorizontalScrollBar()
    {
        if (_hScrollBar is null) return;

        var maxLineLength = _viewModel.GetMaxLineLength();
        var viewportCols = _viewModel.ViewportWidth;
        int maxPosition = Math.Max(0, maxLineLength - viewportCols);

        _hScrollBar.Size = Math.Max(1, maxLineLength);
        _hScrollBar.Position = Math.Clamp(_viewModel.Viewport.HorizontalOffset, 0, maxPosition);
        _hScrollBar.Visible = maxLineLength > viewportCols;
        _hScrollBar.SetNeedsDisplay();
    }

    private bool IsInScrollBarArea(int x, int y) =>
        x >= Bounds.Width - 1 || y >= Bounds.Height - 1;
}
