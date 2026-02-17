using D20Tek.Notepad.Core.Editing;

namespace D20Tek.Notepad.Core.History;

public sealed class UndoStack
{
    private readonly Stack<IUndoableOperation> _undo = new();
    private readonly Stack<IUndoableOperation> _redo = new();
    private List<IUndoableOperation>? _currentGroup = null;

    public void Push(IUndoableOperation op)
    {
        ArgumentNullException.ThrowIfNull(op);

        // if grouping, accumulate operations
        if (_currentGroup != null)
        {
            _currentGroup.Add(op);
            return;
        }

        _undo.Push(op);

        // new edits invalidate redo history
        _redo.Clear();
    }

    public void Undo(EditorSession session)
    {
        if (_undo.Count == 0) return;

        var op = _undo.Pop();
        op.Undo(session);
        _redo.Push(op);
    }

    public void Redo(EditorSession session)
    {
        if (_redo.Count == 0) return;

        var op = _redo.Pop();
        op.Redo(session);
        _undo.Push(op);
    }

    public void BeginGroup()
    {
        if (_currentGroup != null) throw new InvalidOperationException("Undo grouping already active.");
        _currentGroup = [];
    }

    public void EndGroup()
    {
        if (_currentGroup == null) throw new InvalidOperationException("No undo group is active.");

        var group = _currentGroup;
        _currentGroup = null;

        switch (group.Count)
        {
            case 0: break;
            case 1:
                Push(group[0]);
                break;
            default:
                Push(new CompositeOperation(group));
                break;
        }
    }
}
