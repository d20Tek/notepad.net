using D20Tek.Notepad.Tui.Dialogs;

namespace D20Tek.Notepad.Tui.Commands;

internal static class FileLoadHelper
{
    public static bool LoadAndApply(EditorViewModel viewModel, string filePath)
    {
        var factory = new DocumentFactory();
        long fileLength = new FileInfo(filePath).Length;

        return fileLength < viewModel.Settings.LargeFileThresholdBytes
            ? LoadSmallFile(viewModel, factory, filePath)
            : LoadLargeFile(viewModel, factory, filePath);
    }

    private static bool LoadSmallFile(EditorViewModel viewModel, DocumentFactory factory, string filePath)
    {
        try
        {
            var doc = factory.Load(filePath);
            ApplyDocument(viewModel, doc, filePath);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open file:\n{ex.Message}", "OK");
            return false;
        }
    }

    private static bool LoadLargeFile(EditorViewModel viewModel, DocumentFactory factory, string filePath)
    {
        var progressDialog = LoadProgressDialog.Create(Path.GetFileName(filePath));
        IDocument? result = null;
        Exception? error = null;

        Task.Run(() =>
        {
            try
            {
                result = factory.Load(
                    filePath,
                    viewModel.Settings.LargeFileThresholdBytes,
                    progressDialog,
                    progressDialog.Token);
            }
            catch (OperationCanceledException)
            {
                // user cancelled — nothing to report
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                Application.MainLoop.Invoke(() => Application.RequestStop());
            }
        });

        Application.Run(progressDialog);

        if (progressDialog.IsCancelled) return false;

        if (error != null)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open file:\n{error.Message}", "OK");
            return false;
        }

        ApplyDocument(viewModel, result!, filePath);
        return true;
    }

    private static void ApplyDocument(EditorViewModel viewModel, IDocument document, string filePath)
    {
        viewModel.Session.ReplaceDocument(document);
        viewModel.Session.UndoStack.Clear();
        viewModel.SetFilePath(Path.GetFullPath(filePath));
        viewModel.ResetCleanVersion();
        viewModel.Viewport.Reset();
        viewModel.Refresh();
    }
}
