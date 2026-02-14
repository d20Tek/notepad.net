using D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.Storage;

public interface ITextStorageStrategy
{
    DocumentData Load(Stream stream);

    void Save(Stream stream, IDocument document);
}
