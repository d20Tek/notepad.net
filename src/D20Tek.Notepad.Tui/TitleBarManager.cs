namespace D20Tek.Notepad.Tui;

internal sealed class TitleBarManager : IDisposable
{
    private const string AppName = "Notepad.Tui";
    private const string NewFileTitle = "[new-file]";

    private readonly EditorViewModel _viewModel;

    public TitleBarManager(EditorViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.DirtyStateChanged += OnDirtyStateChanged;
        _viewModel.FilePathChanged += OnFilePathChanged;

        UpdateTitle();
    }

    public void Dispose()
    {
        _viewModel.DirtyStateChanged -= OnDirtyStateChanged;
        _viewModel.FilePathChanged -= OnFilePathChanged;
    }

    private void OnDirtyStateChanged(bool _) => UpdateTitle();

    private void OnFilePathChanged(string? _) => UpdateTitle();

    private void UpdateTitle()
    {
        string documentName = GetDocumentName();
        string dirtyIndicator = _viewModel.IsDirty ? "*" : "";

        // Set the terminal window title
        Console.Title = $"{dirtyIndicator}{documentName} - {AppName}";
    }

    private string GetDocumentName() =>
        string.IsNullOrEmpty(_viewModel.CurrentFilePath) ? NewFileTitle : Path.GetFileName(_viewModel.CurrentFilePath);
}

