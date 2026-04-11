namespace D20Tek.Notepad.Tui;

public partial class EditorView
{
    private ScrollBarView? _vScrollBar;
    private ScrollBarView? _hScrollBar;
    private bool _scrollBarsInitialized;

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

        int newSize = Math.Max(1, totalLines);
        int newPosition = Math.Clamp(_viewModel.Viewport.FirstVisibleLine, 0, maxPosition);
        bool newVisible = totalLines > viewportLines;

        // Change guards: Terminal.Gui's Size setter always calls SetNeedsDisplay (no
        // change check), so setting the same value every Redraw creates a continuous
        // redraw cycle that starves input processing.
        if (_vScrollBar.Size != newSize) _vScrollBar.Size = newSize;
        if (_vScrollBar.Position != newPosition) _vScrollBar.Position = newPosition;
        if (_vScrollBar.Visible != newVisible) _vScrollBar.Visible = newVisible;
    }

    private void SyncHorizontalScrollBar()
    {
        if (_hScrollBar is null) return;

        // Hide horizontal scrollbar when word wrap is enabled
        if (_viewModel.IsWordWrapEnabled)
        {
            if (_hScrollBar.Visible) _hScrollBar.Visible = false;
            return;
        }

        var maxLineLength = _viewModel.GetMaxLineLength();
        var viewportCols = _viewModel.ViewportWidth;
        int maxPosition = Math.Max(0, maxLineLength - viewportCols);

        int newSize = Math.Max(1, maxLineLength + 1);
        int newPosition = Math.Clamp(_viewModel.Viewport.HorizontalOffset, 0, maxPosition);
        bool newVisible = maxLineLength > viewportCols;

        if (_hScrollBar.Size != newSize) _hScrollBar.Size = newSize;
        if (_hScrollBar.Position != newPosition) _hScrollBar.Position = newPosition;
        if (_hScrollBar.Visible != newVisible) _hScrollBar.Visible = newVisible;
    }

    private bool IsInScrollBarArea(int x, int y)
    {
        // Only treat as scrollbar area if the scrollbar is actually visible
        bool inVerticalScrollbar = _vScrollBar?.Visible == true && x >= Bounds.Width - 1;
        bool inHorizontalScrollbar = _hScrollBar?.Visible == true && y >= Bounds.Height - 1;
        return inVerticalScrollbar || inHorizontalScrollbar;
    }
}
