namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private string? _currentFilePath;
    private bool _isDirty;

    public event Action<bool>? DirtyStateChanged;
    public event Action<string?>? FilePathChanged;

    public string? CurrentFilePath
    {
        get => _currentFilePath;
        private set
        {
            if (_currentFilePath != value)
            {
                _currentFilePath = value;
                FilePathChanged?.Invoke(value);
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                DirtyStateChanged?.Invoke(value);
            }
        }
    }

    public string DocumentTitle => string.IsNullOrEmpty(CurrentFilePath) ? "Untitled" : Path.GetFileName(CurrentFilePath);

    public void SetFilePath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        CurrentFilePath = filePath;
    }

    public void ClearFilePath() => CurrentFilePath = null;

    public void MarkDirty() => IsDirty = true;

    public void ClearDirtyFlag() => IsDirty = false;
}
