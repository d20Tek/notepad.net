namespace D20Tek.Notepad.Tui;

internal sealed class TerminalGuiClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        Clipboard.TrySetClipboardData(text);
    }

    public string? GetText()
    {
        if (!Clipboard.IsSupported) return null;

        try
        {
            var contents = Clipboard.Contents?.ToString();
            return string.IsNullOrEmpty(contents) ? null : contents;
        }
        catch
        {
            return null;
        }
    }

    // Always report true to avoid Terminal.Gui's TryGetClipboardData infinite loop
    // (its internal GetClipboardDataImpl busy-waits when clipboard returns null).
    // The Paste command itself handles empty clipboard gracefully.
    public bool ContainsText => Clipboard.IsSupported;
}
