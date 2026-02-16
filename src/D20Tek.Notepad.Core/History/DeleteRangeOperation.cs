using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.History;

public sealed class DeleteRangeOperation(
    TextRange range,
    string oldText,
    TextPosition oldCaret) : IUndoableOperation
{
    private readonly ReplaceRangeOperation _inner = new(range, string.Empty, oldText, oldCaret);

    public void Undo(EditorSession session) => _inner.Undo(session);

    public void Redo(EditorSession session) => _inner.Redo(session);
}
