namespace D20Tek.Notepad.Tui;

internal sealed class TerminalGuiClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Clipboard.TrySetClipboardData(text);
    }

    public string? GetText() => Clipboard.TryGetClipboardData(out string? data) ? data : null;

    public bool ContainsText => Clipboard.TryGetClipboardData(out string? data) && !string.IsNullOrEmpty(data);
}
