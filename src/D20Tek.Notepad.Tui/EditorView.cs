using D20Tek.Notepad.Tui.Input;
using static Terminal.Gui.Application;

namespace D20Tek.Notepad.Tui;

public sealed partial class EditorView : View, IDisposable
{
    private readonly EditorViewModel _viewModel;
    private readonly TerminalGuiRenderer _renderer;
    private bool _disposed;

    public event Action? ZoomRequested;

    public EditorView(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _renderer = new TerminalGuiRenderer(this);

        CanFocus = true;
        WantMousePositionReports = true;
        ColorScheme = EditorColorSchemes.Editor;

        Added += OnAddedToSuperView;
        Resized += OnViewResized;

        _viewModel.ViewChanged += OnViewModelChanged;
        _viewModel.CaretMoved += OnViewModelChanged;
        _viewModel.SelectionChanged += OnViewModelChanged;
        _viewModel.WordWrapChanged += OnWordWrapChanged;
        _viewModel.LineNumbersChanged += OnLineNumbersChanged;
        _viewModel.InsertModeChanged += OnInsertModeChanged;
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
        _viewModel.LineNumbersChanged -= OnLineNumbersChanged;
        _viewModel.InsertModeChanged -= OnInsertModeChanged;

        Resized -= OnViewResized;
        Added -= OnAddedToSuperView;
    }

    public override void Redraw(Rect bounds)
    {
        UpdateViewportSize();
        SyncScrollBarState();

        _viewModel.RenderFrame(_renderer);
        if (_viewModel.IsOverwriteMode && HasFocus && !_viewModel.Session.HasSelection)
            DrawOverwriteBlockCursor();
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
            var cursorStyle = _viewModel.IsOverwriteMode && !_viewModel.Session.HasSelection
                ? CursorVisibility.Invisible
                : CursorVisibility.VerticalFix;
            Application.Driver.SetCursorVisibility(cursorStyle);
            Application.Driver.Move(screenX, screenY);
        }
        else
        {
            Application.Driver.SetCursorVisibility(CursorVisibility.Invisible);
        }
    }

    public override bool MouseEvent(MouseEvent me)
    {
        var flags = me.Flags;

        // Ctrl+ScrollWheel → zoom hint (intercept before plain scroll in MouseBindings)
        if (flags.HasFlag(MouseFlags.ButtonCtrl) &&
            (flags.HasFlag(MouseFlags.WheeledUp) || flags.HasFlag(MouseFlags.WheeledDown)))
        {
            ZoomRequested?.Invoke();
            return true;
        }

        // Only handle mouse events that are within our bounds
        if (me.X < 0 || me.Y < 0 || me.X >= Bounds.Width || me.Y >= Bounds.Height) return false;

        if (IsInScrollBarArea(me.X, me.Y)) return base.MouseEvent(me);

        if (TryHandleGutterEvent(me)) return true;

        me.X -= _viewModel.GutterWidth;

        if (MouseBindings.TryExecute(_viewModel, me))
        {
            SetNeedsDisplay();
            return true;
        }

        return base.MouseEvent(me);
    }

    private bool TryHandleGutterEvent(MouseEvent me)
    {
        int gutterWidth = _viewModel.GutterWidth;
        if (gutterWidth == 0 || me.X >= gutterWidth) return false;

        var flags = me.Flags;

        // Wheel scroll passes through unchanged (ScrollWheel uses no coordinates)
        if (flags.HasFlag(MouseFlags.WheeledUp) || flags.HasFlag(MouseFlags.WheeledDown)) return false;

        // Drag into gutter passes through; adjusted X will be negative, handled by CalculateColumn's left-edge path
        if (flags.HasFlag(MouseFlags.Button1Pressed) && flags.HasFlag(MouseFlags.ReportMousePosition)) return false;

        // Single click in gutter selects the entire line at that row
        if (flags.HasFlag(MouseFlags.Button1Clicked))
        {
            int line = MouseBindings.CalculateDocumentLine(_viewModel, me.Y);
            _viewModel.SelectLineAt(line);
            SetNeedsDisplay();
        }

        return true;
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

    private void OnLineNumbersChanged(bool _) => SetNeedsDisplay();

    private void OnInsertModeChanged(bool _) => SetNeedsDisplay();

    private void DrawOverwriteBlockCursor()
    {
        if (!IsCaretVisible()) return;

        var caret = _viewModel.CaretViewPosition;
        var lines = _viewModel.VisibleLines;
        if (caret.LineIndex < 0 || caret.LineIndex >= lines.Count) return;

        var lineText = lines[caret.LineIndex].Text;
        string ch = caret.Column < lineText.Length ? lineText[caret.Column].ToString() : " ";

        var (screenX, screenY) = GetCaretScreenPosition();
        var normal = ColorScheme.Normal;

        Application.Driver.SetAttribute(new Terminal.Gui.Attribute(normal.Background, normal.Foreground));
        Application.Driver.Move(screenX, screenY);
        Application.Driver.AddStr(ch);
        Application.Driver.SetAttribute(normal);
    }

    private void OnViewResized(ResizedEventArgs args)
    {
        // Initial resize - scrollbars may not exist yet, so use full dimensions
        // UpdateViewportSize in Redraw will adjust once scrollbar visibility is known
        _viewModel.SetViewportWidth(Math.Max(0, Frame.Width - _viewModel.GutterWidth));
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
            _viewModel.SetViewportWidth(Math.Max(0, effectiveWidth - _viewModel.GutterWidth));
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
               caretViewPos.Column <= _viewModel.ViewportWidth;
    }

    private (int X, int Y) GetCaretScreenPosition()
    {
        var caretViewPos = _viewModel.CaretViewPosition;

        // Clamp column so a caret at the end of a full-width wrapped segment stays on-screen
        int effectiveColumn = _viewModel.ViewportWidth > 0
            ? Math.Min(caretViewPos.Column, _viewModel.ViewportWidth - 1)
            : 0;

        int screenX = effectiveColumn + Frame.X + _viewModel.GutterWidth;
        int screenY = caretViewPos.LineIndex + Frame.Y;

        return (screenX, screenY);
    }
}
