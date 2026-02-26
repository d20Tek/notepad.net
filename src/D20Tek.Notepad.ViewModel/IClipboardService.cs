namespace D20Tek.Notepad.ViewModel;

public interface IClipboardService
{
    void SetText(string text);

    string? GetText();

    bool ContainsText { get; }
}
