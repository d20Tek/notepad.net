namespace D20Tek.Notepad.Core.Document;

public interface IDocumentFactory
{
    IDocument Create(DocumentData data);

    IDocument Empty { get; }

    IDocument Load(string filePath);

    public void Save(IDocument document, string filePath);
}
