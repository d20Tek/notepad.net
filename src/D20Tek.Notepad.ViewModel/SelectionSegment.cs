namespace D20Tek.Notepad.ViewModel;

public readonly struct SelectionSegment(int startColumn, int endColumn)
{
    public int StartColumn { get; } = startColumn;

    public int EndColumn { get; } = endColumn;

    public override string ToString() => $"[{StartColumn}..{EndColumn}]";
}
