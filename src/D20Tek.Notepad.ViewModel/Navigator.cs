namespace D20Tek.Notepad.ViewModel;

public sealed class Navigator(EditorViewModel viewModel)
{
    private readonly EditorViewModel _viewModel = viewModel;

    public void MoveCaretLeft() => ExecuteNavigation(_viewModel.Commands.MoveLeft);

    public void MoveCaretRight() => ExecuteNavigation(_viewModel.Commands.MoveRight);

    public void MoveCaretUp() => ExecuteNavigation(_viewModel.Commands.MoveUp);

    public void MoveCaretDown() => ExecuteNavigation(_viewModel.Commands.MoveDown);

    public void MoveToLineStart() => ExecuteNavigation(_viewModel.Commands.MoveToLineStart);

    public void MoveToLineEnd() => ExecuteNavigation(_viewModel.Commands.MoveToLineEnd);

    public void MoveToDocumentStart() => ExecuteNavigation(_viewModel.Commands.MoveToDocumentStart);

    public void MoveToDocumentEnd() => ExecuteNavigation(_viewModel.Commands.MoveToDocumentEnd);

    private void ExecuteNavigation(Action navAction)
    {
        navAction();
        _viewModel.EnsureCaretVisible();
    }
}

