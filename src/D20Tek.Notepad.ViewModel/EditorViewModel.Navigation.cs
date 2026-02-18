namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public void MoveCaretLeft() =>
        ExecuteNavigation(() => Commands.MoveLeft());

    public void MoveCaretRight() =>
        ExecuteNavigation(() => Commands.MoveRight());

    public void MoveCaretUp() =>
        ExecuteNavigation(() => Commands.MoveUp());

    public void MoveCaretDown() =>
        ExecuteNavigation(() => Commands.MoveDown());

    public void MoveToLineStart() =>
        ExecuteNavigation(() => Commands.MoveToLineStart());

    public void MoveToLineEnd() =>
        ExecuteNavigation(() => Commands.MoveToLineEnd());

    public void MoveToDocumentStart() =>
        ExecuteNavigation(() => Commands.MoveToDocumentStart());

    public void MoveToDocumentEnd() =>
        ExecuteNavigation(() => Commands.MoveToDocumentEnd());

    private void ExecuteNavigation(Action navAction)
    {
        navAction();
        EnsureCaretVisible();
    }
}

