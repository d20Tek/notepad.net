using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.History;

internal sealed class InsertTextOperation(
    TextPosition position,
    string text,
    string oldText,
    TextPosition oldCaret,
    TextPosition newCaret) : IUndoableOperation
{
    private readonly ReplaceRangeOperation _inner = new(new TextRange(position, position), text, oldText, oldCaret, newCaret);

    public void Undo(EditorSession session) => _inner.Undo(session);

    public void Redo(EditorSession session) => _inner.Redo(session);
}
