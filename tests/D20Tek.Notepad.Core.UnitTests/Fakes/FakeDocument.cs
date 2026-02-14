using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.UnitTests.Fakes;

internal sealed class FakeDocument(params string[] lines) : IDocument
{
    public int LineCount => Lines.Count;

    public IList<TextLine> Lines { get; } = [.. lines.Select(l => new TextLine(l))];
}
