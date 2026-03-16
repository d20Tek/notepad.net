namespace D20Tek.Notepad.ViewModel;

public sealed partial class EditorViewModel
{
    public event Action<IReadOnlyList<string>>? RecentFilesChanged;

    public IReadOnlyList<string> RecentFiles => _settings.RecentFiles;

    internal void HookFilePathChanged() => FilePathChanged += OnFilePathChanged;

    private void OnFilePathChanged(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var updated = new MruList { Paths = _settings.RecentFiles }.Add(path);
        _settings = _settings with { RecentFiles = updated.Paths };
        RecentFilesChanged?.Invoke(_settings.RecentFiles);
    }
}
