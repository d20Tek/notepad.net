using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed class EditorNavigationService
{
    // Basic caret movement
    public void MoveLeft(EditorSession session) => CaretMovementHelper.MoveLeft(session);

    public void MoveRight(EditorSession session) => CaretMovementHelper.MoveRight(session);

    public void MoveUp(EditorSession session) => CaretMovementHelper.MoveUp(session);

    public void MoveDown(EditorSession session) => CaretMovementHelper.MoveDown(session);

    // Line/document boundary movement
    public void MoveToLineStart(EditorSession session)
    {
        session.Caret = new TextPosition(session.Caret.Line, 0);
        session.Anchor = session.Caret;
    }

    public void MoveToLineEnd(EditorSession session)
    {
        var line = session.Document.Lines[session.Caret.Line];
        session.Caret = new TextPosition(session.Caret.Line, line.Content.Length);
        session.Anchor = session.Caret;
    }

    public void MoveToDocumentStart(EditorSession session)
    {
        session.Caret = new TextPosition(0, 0);
        session.Anchor = session.Caret;
    }

    public void MoveToDocumentEnd(EditorSession session)
    {
        int lastLineIndex = session.Document.Lines.Count - 1;
        var lastLine = session.Document.Lines[lastLineIndex];

        session.Caret = new TextPosition(lastLineIndex, lastLine.Content.Length);
        session.Anchor = session.Caret;
    }

    // Selection extension
    public void ExtendLeft(EditorSession session)
    {
        var oldAnchor = session.Anchor;
        SelectionMovementHelper.MoveLeft(session);
        session.Anchor = oldAnchor;
    }

    public void ExtendRight(EditorSession session)
    {
        var oldAnchor = session.Anchor;
        SelectionMovementHelper.MoveRight(session);
        session.Anchor = oldAnchor;
    }

    public void ExtendUp(EditorSession session)
    {
        var oldAnchor = session.Anchor;
        SelectionMovementHelper.MoveUp(session);
        session.Anchor = oldAnchor;
    }

    public void ExtendDown(EditorSession session)
    {
        var oldAnchor = session.Anchor;
        SelectionMovementHelper.MoveDown(session);
        session.Anchor = oldAnchor;
    }
}
