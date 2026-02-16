using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed class EditorSession(IDocument document, TextPosition caret, TextPosition anchor)
{
    public IDocument Document { get; } = document;

    public TextPosition Caret { get; set; } = caret;

    public TextPosition Anchor { get; set; } = anchor;


    internal TextPosition ApplyReplaceRange(TextRange range, string replacement)
    {
        var newCaret = RangeEditingService.ReplaceRange(Document, range, replacement);

        Caret = newCaret;
        Anchor = newCaret;

        return newCaret;
    }
}
