namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private bool _isOverwriteMode = false;

    public bool IsOverwriteMode => _isOverwriteMode;

    public event Action<bool>? InsertModeChanged;

    public void ToggleInsertMode()
    {
        _isOverwriteMode = !_isOverwriteMode;
        Refresh();
        InsertModeChanged?.Invoke(_isOverwriteMode);
    }
}
