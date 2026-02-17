using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

internal static class CaretMovementHelper
{
    public static void MoveLeft(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Column > 0)
        {
            session.Caret = new TextPosition(caret.Line, caret.Column - 1);
            session.Anchor = session.Caret;
            return;
        }

        if (caret.Line > 0)
        {
            var prevLine = session.Document.Lines[caret.Line - 1];
            session.Caret = new TextPosition(caret.Line - 1, prevLine.Content.Length);
            session.Anchor = session.Caret;
        }
    }

    public static void MoveRight(EditorSession session)
    {
        var caret = session.Caret;
        var line = session.Document.Lines[caret.Line];

        if (caret.Column < line.Content.Length)
        {
            session.Caret = new TextPosition(caret.Line, caret.Column + 1);
            session.Anchor = session.Caret;
            return;
        }

        if (caret.Line < session.Document.Lines.Count - 1)
        {
            session.Caret = new TextPosition(caret.Line + 1, 0);
            session.Anchor = session.Caret;
        }
    }

    public static void MoveUp(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Line == 0)
        {
            session.Caret = new TextPosition(0, 0);
            session.Anchor = session.Caret;
            return;
        }

        var prevLine = session.Document.Lines[caret.Line - 1];
        int newCol = Math.Min(caret.Column, prevLine.Content.Length);

        session.Caret = new TextPosition(caret.Line - 1, newCol);
        session.Anchor = session.Caret;
    }

    public static void MoveDown(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Line == session.Document.Lines.Count - 1)
        {
            var lastLine = session.Document.Lines[caret.Line];
            session.Caret = new TextPosition(caret.Line, lastLine.Content.Length);
            session.Anchor = session.Caret;
            return;
        }

        var nextLine = session.Document.Lines[caret.Line + 1];
        int newCol = Math.Min(caret.Column, nextLine.Content.Length);

        session.Caret = new TextPosition(caret.Line + 1, newCol);
        session.Anchor = session.Caret;
    }
}
