namespace D20Tek.Notepad.ViewModel.UnitTests;

[ExcludeFromCodeCoverage]
internal sealed class MockClipboardService : IClipboardService
{
    private string? _clipboardText;

    public void SetText(string text) => _clipboardText = text;

    public string? GetText() => _clipboardText;

    public bool ContainsText => !string.IsNullOrEmpty(_clipboardText);

    public void Clear() => _clipboardText = null;
}
