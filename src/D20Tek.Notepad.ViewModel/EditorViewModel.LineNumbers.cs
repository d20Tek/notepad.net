namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public event Action<bool>? LineNumbersChanged;

    public bool IsLineNumbersEnabled => _settings.LineNumbersEnabled;

    public int GutterWidth => _settings.LineNumbersEnabled ? Session.Document.Lines.Count.ToString().Length + 2 : 0;

    public void ToggleLineNumbers()
    {
        _settings = _settings with { LineNumbersEnabled = !_settings.LineNumbersEnabled };
        LineNumbersChanged?.Invoke(_settings.LineNumbersEnabled);
    }
}
