namespace D20Tek.Notepad.Core.Document;

public interface IDocumentFactory
{
    IDocument Create(DocumentData data);
}
