namespace D20Tek.Notepad.Tui;

internal sealed class StatusBarView : View, IDisposable
{
    private const int DebounceMilliseconds = 50;

    private readonly EditorViewModel _viewModel;
    private StatusDetails _status;
    private bool _disposed;
    private bool _pendingRedraw;

    public StatusBarView(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _status = viewModel.CurrentStatus;

        X = 0;
        Y = Pos.AnchorEnd(1);
        Width = Dim.Fill();
        Height = 1;
        CanFocus = false;
        ColorScheme = EditorColorSchemes.StatusBar;

        _viewModel.StatusChanged += OnStatusChanged;
    }

    public new void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _viewModel.StatusChanged -= OnStatusChanged;
    }

    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(ColorScheme.Normal);

        Move(0, 0);
        Driver.AddStr(new string(' ', bounds.Width));

        string left = FormatPosition(_status);
        string center = FormatStatistics(_status);
        string right = FormatEncoding(_status);

        // Left section (with 1-space margin)
        Move(1, 0);
        Driver.AddStr(left);

        // Right section (right-aligned, with 1-space margin)
        int rightX = bounds.Width - right.Length - 1;

        // Center section (centered between left and right)
        int centerX = (bounds.Width - center.Length) / 2;

        if (rightX > 0 && centerX > left.Length + 2 && centerX + center.Length < rightX)
        {
            Move(centerX, 0);
            Driver.AddStr(center);

            Move(rightX, 0);
            Driver.AddStr(right);
        }
        else if (rightX > 0)
        {
            Move(rightX, 0);
            Driver.AddStr(right);
        }
    }

    private void OnStatusChanged(StatusDetails status)
    {
        _status = status;

        if (_pendingRedraw) return;

        _pendingRedraw = true;
        Application.MainLoop?.AddTimeout(TimeSpan.FromMilliseconds(DebounceMilliseconds), _ =>
        {
            _pendingRedraw = false;
            if (!_disposed) SetNeedsDisplay();
            return false;
        });
    }

    private static string FormatPosition(StatusDetails s) => $"Ln {s.CurrentLine}, Col {s.CurrentColumn}";

    private static string FormatStatistics(StatusDetails s) =>
        $"Lines: {s.TotalLines:N0} | Chars: {s.TotalCharacters:N0}";

    private static string FormatEncoding(StatusDetails s) => $"{s.DocumentEncoding} {s.LineEndingStyle}";
}
