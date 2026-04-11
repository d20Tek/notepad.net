using D20Tek.Notepad.Core.Storage;

namespace D20Tek.Notepad.Tui.Dialogs;

internal sealed class LoadProgressDialog : Dialog, ILoadProgress
{
    private readonly ProgressBar _progressBar;
    private readonly CancellationTokenSource _cts = new();
    private float _lastReportedFraction = -1f;

    public CancellationToken Token => _cts.Token;

    public bool IsCancelled => _cts.IsCancellationRequested;

    private LoadProgressDialog(string fileName)
    {
        Title = "Loading Large File";
        Width = 52;
        Height = 8;

        var messageLabel = new Label($"Loading {fileName}...") { X = 1, Y = 1 };
        _progressBar = new ProgressBar { X = 1, Y = 3, Width = Dim.Fill() - 2, Fraction = 0.0f };
        var cancelButton = new Button("Cancel") { X = Pos.Center(), Y = 5 };
        cancelButton.Clicked += () =>
        {
            _cts.Cancel();
            Application.RequestStop();
        };

        Add(messageLabel, _progressBar, cancelButton);
    }

    public void Report(long bytesProcessed, long totalBytes)
    {
        if (totalBytes <= 0) return;
        float fraction = Math.Clamp((float)bytesProcessed / totalBytes, 0.0f, 1.0f);

        if (Math.Abs(fraction - _lastReportedFraction) < 0.005f) return;
        _lastReportedFraction = fraction;

        Application.MainLoop.Invoke(() =>
        {
            _progressBar.Fraction = fraction;
            Application.Refresh();
        });
    }

    public static LoadProgressDialog Create(string fileName) => new(fileName);
}
