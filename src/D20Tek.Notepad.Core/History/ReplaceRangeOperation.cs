using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.History;

public sealed class ReplaceRangeOperation(
    TextRange range,
    string newText,
    string oldText,
    TextPosition oldCaret,
    TextPosition newCaret) : IUndoableOperation
{
    private readonly TextRange _range = range;
    private readonly string _newText = newText ?? string.Empty;
    private readonly string _oldText = oldText ?? string.Empty;
    private readonly TextPosition _oldCaret = oldCaret;
    private readonly TextPosition _newCaret = newCaret;

    public void Undo(EditorSession session)
    {
        session.ApplyReplaceRange(
            new TextRange(_range.Start, _range.Start.AdvanceColumn(_newText.Length)), _oldText);

        session.Caret = _oldCaret;
        session.Anchor = _oldCaret;
    }

    public void Redo(EditorSession session)
    {
        session.ApplyReplaceRange(_range, _newText);
        session.Caret = _newCaret;
        session.Anchor = _newCaret;
    }
}
