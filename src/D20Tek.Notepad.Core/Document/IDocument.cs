using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Document;

public interface IDocument
{
    public int LineCount { get; }

    public IList<TextLine> Lines { get; }
}
