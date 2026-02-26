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
