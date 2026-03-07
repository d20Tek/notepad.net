using D20Tek.Notepad.Core.Editing;
using System.Text;

namespace D20Tek.Notepad.Core.History;

internal sealed class ChangeEncodingOperation(Encoding previousEncoding, Encoding newEncoding) : IUndoableOperation
{
    public void Undo(EditorSession session) => session.Document.SetEncoding(previousEncoding);

    public void Redo(EditorSession session) => session.Document.SetEncoding(newEncoding);
}
