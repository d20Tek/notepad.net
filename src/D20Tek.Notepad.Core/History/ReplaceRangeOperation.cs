using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.History;

public sealed class ReplaceRangeOperation(TextRange range, string newText, string oldText, TextPosition oldCaret)
    : IUndoableOperation
{
    private readonly TextRange _range = range;
    private readonly string _newText = newText ?? string.Empty;
    private readonly string _oldText = oldText ?? string.Empty;
    private readonly TextPosition _oldCaret = oldCaret;

    private bool _hasExecuted;
    private TextPosition _newCaret;

    public void Undo(EditorSession session)
    {
        var end = AdvancePosition(_range.Start, _newText);
        var insertedRange = new TextRange(_range.Start, end);

        session.ApplyReplaceRange(insertedRange, _oldText);

        session.Caret = _oldCaret;
        session.Anchor = _oldCaret;
    }

    public void Redo(EditorSession session)
    {
        var caret = session.ApplyReplaceRange(_range, _newText);

        if (!_hasExecuted)
        {
            _newCaret = caret;
            _hasExecuted = true;
        }

        session.Caret = _newCaret;
        session.Anchor = _newCaret;
    }

    private static TextPosition AdvancePosition(TextPosition start, string text)
    {
        var lines = text.Split(Environment.NewLine);

        if (lines.Length == 1)
        {
            return new TextPosition(start.Line, start.Column + lines[0].Length);
        }

        int newLine = start.Line + (lines.Length - 1);
        int newColumn = lines[^1].Length;
        return new TextPosition(newLine, newColumn);
    }
}
