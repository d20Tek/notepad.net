using System.Diagnostics.CodeAnalysis;

namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    private string? _currentFilePath;
    private int _cleanVersion; 
    private bool _lastDirtyState; 

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

    public bool IsDirty => Session.UndoStack.Version != _cleanVersion || 
                           Session.UndoStack.HasPendingGroupOperations;

    public string DocumentTitle => string.IsNullOrEmpty(CurrentFilePath) ? "Untitled" : Path.GetFileName(CurrentFilePath);

    public void SetFilePath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        CurrentFilePath = filePath;
    }

    public void ClearFilePath() => CurrentFilePath = null;

    internal void CheckDirtyStateChanged()
    {
        bool currentDirty = IsDirty;
        if (_lastDirtyState != currentDirty)
        {
            _lastDirtyState = currentDirty;
            DirtyStateChanged?.Invoke(currentDirty);
        }
    }

    public void ClearDirtyFlag()
    {
        EndTypingGroupIfNeeded(); // Ensure any pending typing is committed
        _cleanVersion = Session.UndoStack.Version;
        CheckDirtyStateChanged();
    }

    public void ResetCleanVersion()
    {
        _cleanVersion = 0;
        _lastDirtyState = false;
    }
}
