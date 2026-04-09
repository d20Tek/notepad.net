using D20Tek.Notepad.Core.Storage;

namespace D20Tek.Notepad.Core.Document;

public interface IDocumentFactory
{
    IDocument Create(DocumentData data);

    IDocument Empty { get; }

    IDocument Load(
        string filePath,
        long largeFileThreshold = 10_485_760,
        ILoadProgress? progress = null,
        CancellationToken cancellation = default);

    public void Save(IDocument document, string filePath);
}
