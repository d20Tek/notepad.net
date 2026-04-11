namespace D20Tek.Notepad.ViewModel;

public partial class EditorViewModel
{
    public void Cut()
    {
        if (!Session.HasSelection || _clipboardService is null) return;
        EndTypingGroupIfNeeded();

        var text = Commands.CutSelection();
        _clipboardService.SetText(text);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public void Copy()
    {
        if (!Session.HasSelection || _clipboardService is null) return;

        var text = Commands.CopySelection();
        _clipboardService.SetText(text);
    }

    public void Paste()
    {
        if (_clipboardService is null) return;

        var text = _clipboardService.GetText();
        if (string.IsNullOrEmpty(text)) return;
        EndTypingGroupIfNeeded();

        Commands.Paste(text);
        CheckDirtyStateChanged();
        EnsureCaretVisible();
    }

    public bool CanCut => Session.HasSelection;

    public bool CanCopy => Session.HasSelection;

    public bool CanPaste => _clipboardService?.ContainsText ?? false;
}
