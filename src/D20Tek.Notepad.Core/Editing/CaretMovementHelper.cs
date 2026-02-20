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

    public static void MovePageUp(EditorSession session, int pageHeight)
    {
        var caret = session.Caret;

        // If already at the top, clamp to (0, 0)
        if (caret.Line == 0)
        {
            session.Caret = new TextPosition(0, 0);
            session.Anchor = session.Caret;
            return;
        }

        // Compute target line
        int targetLine = Math.Max(0, caret.Line - pageHeight);

        var target = session.Document.Lines[targetLine];
        int newCol = Math.Min(caret.Column, target.Content.Length);

        session.Caret = new TextPosition(targetLine, newCol);
        session.Anchor = session.Caret;
    }

    public static void MovePageDown(EditorSession session, int pageHeight)
    {
        var caret = session.Caret;
        int lastLineIndex = session.Document.Lines.Count - 1;

        // If already at the bottom, clamp to end of last line
        if (caret.Line == lastLineIndex)
        {
            var lastLine = session.Document.Lines[lastLineIndex];
            session.Caret = new TextPosition(lastLineIndex, lastLine.Content.Length);
            session.Anchor = session.Caret;
            return;
        }

        // Compute target line
        int targetLine = Math.Min(lastLineIndex, caret.Line + pageHeight);

        var target = session.Document.Lines[targetLine];
        int newCol = Math.Min(caret.Column, target.Content.Length);

        session.Caret = new TextPosition(targetLine, newCol);
        session.Anchor = session.Caret;
    }
}
