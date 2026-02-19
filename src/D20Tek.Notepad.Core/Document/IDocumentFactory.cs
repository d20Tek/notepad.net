namespace D20Tek.Notepad.Core.Document;

public interface IDocumentFactory
{
    IDocument Create(DocumentData data);

    IDocument Load(string filePath);
}
