using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

internal static class SelectionMovementHelper
{
    public static void MoveLeft(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Column > 0)
        {
            session.Caret = new TextPosition(caret.Line, caret.Column - 1);
            return;
        }

        if (caret.Line > 0)
        {
            var prevLine = session.Document.Lines[caret.Line - 1];
            session.Caret = new TextPosition(caret.Line - 1, prevLine.Content.Length);
        }
    }

    public static void MoveRight(EditorSession session)
    {
        var caret = session.Caret;
        var line = session.Document.Lines[caret.Line];

        if (caret.Column < line.Content.Length)
        {
            session.Caret = new TextPosition(caret.Line, caret.Column + 1);
            return;
        }

        if (caret.Line < session.Document.Lines.Count - 1)
        {
            session.Caret = new TextPosition(caret.Line + 1, 0);
        }
    }

    public static void MoveUp(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Line == 0)
        {
            session.Caret = new TextPosition(0, 0);
            return;
        }

        var prevLine = session.Document.Lines[caret.Line - 1];
        int newCol = Math.Min(caret.Column, prevLine.Content.Length);

        session.Caret = new TextPosition(caret.Line - 1, newCol);
    }

    public static void MoveDown(EditorSession session)
    {
        var caret = session.Caret;

        if (caret.Line == session.Document.Lines.Count - 1)
        {
            var lastLine = session.Document.Lines[caret.Line];
            session.Caret = new TextPosition(caret.Line, lastLine.Content.Length);
            return;
        }

        var nextLine = session.Document.Lines[caret.Line + 1];
        int newCol = Math.Min(caret.Column, nextLine.Content.Length);

        session.Caret = new TextPosition(caret.Line + 1, newCol);
    }

    public static void MovePageUp(EditorSession session, int pageHeight)
    {
        var caret = session.Caret;

        // Already at the top → clamp to (0,0)
        if (caret.Line == 0)
        {
            session.Caret = new TextPosition(0, 0);
            return;
        }

        int targetLine = Math.Max(0, caret.Line - pageHeight);
        var target = session.Document.Lines[targetLine];

        int newCol = Math.Min(caret.Column, target.Content.Length);

        session.Caret = new TextPosition(targetLine, newCol);
    }

    public static void MovePageDown(EditorSession session, int pageHeight)
    {
        var caret = session.Caret;
        int lastLineIndex = session.Document.Lines.Count - 1;

        // Already at bottom → clamp to end of last line
        if (caret.Line == lastLineIndex)
        {
            var lastLine = session.Document.Lines[lastLineIndex];
            session.Caret = new TextPosition(lastLineIndex, lastLine.Content.Length);
            return;
        }

        int targetLine = Math.Min(lastLineIndex, caret.Line + pageHeight);
        var target = session.Document.Lines[targetLine];

        int newCol = Math.Min(caret.Column, target.Content.Length);

        session.Caret = new TextPosition(targetLine, newCol);
    }
}
