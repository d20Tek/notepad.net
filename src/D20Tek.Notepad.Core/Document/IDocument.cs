using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.Core.Document;

public interface IDocument
{
    IReadOnlyList<TextLine> Lines { get; }

    Encoding Encoding { get; }

    LineEndingStyle LineEndingStyle { get; }

    bool IsModified { get; }

    int LineCount { get; }

    void ReplaceLines(int startIndex, int count, IEnumerable<TextLine> newLines);

    void SetEncoding(Encoding encoding);

    void SetLineEndingStyle(LineEndingStyle style);
}
