namespace D20Tek.Notepad.Core.Storage;

public interface ILoadProgress
{
    void Report(long bytesProcessed, long totalBytes);

    static ILoadProgress None { get; } = new NullLoadProgress();

    private sealed class NullLoadProgress : ILoadProgress
    {
        public void Report(long bytesProcessed, long totalBytes) { }
    }
}
