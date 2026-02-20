namespace D20Tek.Notepad.ViewModel;

public sealed class Navigator(EditorViewModel viewModel)
{
    private readonly EditorViewModel _viewModel = viewModel;

    public void MoveCaretLeft() => ExecuteNavigation(_viewModel.Commands.MoveLeft);

    public void MoveCaretRight() => ExecuteNavigation(_viewModel.Commands.MoveRight);

    public void MoveCaretUp() => ExecuteNavigation(_viewModel.Commands.MoveUp);

    public void MoveCaretDown() => ExecuteNavigation(_viewModel.Commands.MoveDown);

    public void MovePageUp() => ExecuteNavigation(() =>
        _viewModel.Commands.PageUp(_viewModel.Viewport.VisibleLineCount));

    public void MovePageDown() => ExecuteNavigation(() =>
        _viewModel.Commands.PageDown(_viewModel.Viewport.VisibleLineCount));

    public void MoveToLineStart() => ExecuteNavigation(_viewModel.Commands.MoveToLineStart);

    public void MoveToLineEnd() => ExecuteNavigation(_viewModel.Commands.MoveToLineEnd);

    public void MoveToDocumentStart() => ExecuteNavigation(_viewModel.Commands.MoveToDocumentStart);

    public void MoveToDocumentEnd() => ExecuteNavigation(_viewModel.Commands.MoveToDocumentEnd);

    public void ExtendSelectionLeft() => ExecuteNavigation(_viewModel.Commands.SelectLeft);

    public void ExtendSelectionRight() => ExecuteNavigation(_viewModel.Commands.SelectRight);

    public void ExtendSelectionUp() => ExecuteNavigation(_viewModel.Commands.SelectUp);

    public void ExtendSelectionDown() => ExecuteNavigation(_viewModel.Commands.SelectDown);

    public void ExtendSelectionToDocumentStart()
    {
        _viewModel.Session.EnsureAnchorExists();
        var anchor = _viewModel.Session.Anchor;
        MoveToDocumentEnd();
        _viewModel.Session.Anchor = anchor;
    }


    public void ExtendSelectionToDocumentEnd()
    {
        _viewModel.Session.EnsureAnchorExists();
        var anchor = _viewModel.Session.Anchor;
        MoveToDocumentEnd();
        _viewModel.Session.Anchor = anchor;
    }

    public void ExtendSelectionPageUp() => ExecuteNavigation(() =>
        _viewModel.Commands.SelectPageUp(_viewModel.Viewport.VisibleLineCount));

    public void ExtendSelectionPageDown() => ExecuteNavigation(() =>
        _viewModel.Commands.SelectPageDown(_viewModel.Viewport.VisibleLineCount));

    public void ExtendSelectionToLineStart()
    {
        _viewModel.Session.EnsureAnchorExists();
        var anchor = _viewModel.Session.Anchor;
        MoveToLineStart();
        _viewModel.Session.Anchor = anchor;
    }

    public void ExtendSelectionToLineEnd()
    {
        _viewModel.Session.EnsureAnchorExists();
        var anchor = _viewModel.Session.Anchor;
        MoveToLineEnd();
        _viewModel.Session.Anchor = anchor;
    }

    private void ExecuteNavigation(Action navAction)
    {
        navAction();
        _viewModel.EnsureCaretVisible();
    }
}

