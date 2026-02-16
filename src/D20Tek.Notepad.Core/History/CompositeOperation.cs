using D20Tek.Notepad.Core.Editing;

namespace D20Tek.Notepad.Core.History;

public sealed class CompositeOperation(IEnumerable<IUndoableOperation> operations) : IUndoableOperation
{
    private readonly List<IUndoableOperation> _operations = [.. operations];

    public void Undo(EditorSession session)
    {
        // undo in reverse order
        for (int i = _operations.Count - 1; i >= 0; i--)
        {
            _operations[i].Undo(session);
        }
    }

    public void Redo(EditorSession session)
    {
        // redo in forward order
        foreach (var op in _operations)
        {
            op.Redo(session);
        }
    }
}
