using D20Tek.Notepad.Core.Editing;

namespace D20Tek.Notepad.Core.History;

public interface IUndoableOperation
{
    void Undo(EditorSession session);

    void Redo(EditorSession session);
}
