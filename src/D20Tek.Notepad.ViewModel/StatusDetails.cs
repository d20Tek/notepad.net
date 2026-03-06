namespace D20Tek.Notepad.ViewModel;

public record StatusDetails(
    int CurrentLine,
    int CurrentColumn,
    int TotalLines,
    int TotalCharacters,
    string DocumentEncoding,
    string LineEndingStyle)
{
    public static StatusDetails Empty { get; } = new(1, 1, 1, 0, "UTF-8", "CRLF");
}
