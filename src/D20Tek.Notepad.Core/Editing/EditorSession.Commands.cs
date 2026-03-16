using D20Tek.Notepad.Core.History;
using D20Tek.Notepad.Core.Primitives;
using System.Text;

namespace D20Tek.Notepad.Core.Editing;

public sealed partial class EditorSession
{
    public void InsertText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (HasSelection)
        {
            ReplaceSelection(text);
            return;
        }

        var op = new InsertTextOperation(Caret, text, string.Empty, Caret);
        Execute(op);
    }

    public void ReplaceSelection(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var range = GetSelectionRange();
        var oldText = GetTextInRange(range);

        var op = new ReplaceRangeOperation(range, text, oldText, Caret);
        Execute(op);
    }

    public void DeleteSelection()
    {
        if (!HasSelection) return;

        var range = GetSelectionRange();
        var oldText = GetTextInRange(range);

        var op = new DeleteRangeOperation(range, oldText, Caret);
        Execute(op);
    }

    public void Backspace()
    {
        if (HasSelection)
        {
            DeleteSelection();
            return;
        }

        if (Caret.Line == 0 && Caret.Column == 0) return;

        TextPosition start;
        TextPosition end = Caret;

        if (Caret.Column > 0)
        {
            start = new TextPosition(Caret.Line, Caret.Column - 1);
        }
        else
        {
            var prevLineIndex = Caret.Line - 1;
            var prevLine = Document.Lines[prevLineIndex];
            start = new TextPosition(prevLineIndex, prevLine.Content.Length);
        }

        var range = new TextRange(start, end).Normalized();
        var oldText = GetTextInRange(range);

        var op = new DeleteRangeOperation(range, oldText, Caret);
        Execute(op);
    }

    public void Delete()
    {
        if (HasSelection)
        {
            DeleteSelection();
            return;
        }

        var line = Document.Lines[Caret.Line];

        bool atLastLine = Caret.Line == Document.Lines.Count - 1;
        bool atEndOfLine = Caret.Column >= line.Content.Length;

        if (atLastLine && atEndOfLine) return;

        TextPosition end = (Caret.Column < line.Content.Length)
            ? new TextPosition(Caret.Line, Caret.Column + 1)
            : new TextPosition(Caret.Line + 1, 0);

        var range = new TextRange(Caret, end).Normalized();
        var oldText = GetTextInRange(range);

        var op = new DeleteRangeOperation(range, oldText, Caret);
        Execute(op);
    }

    public void OverwriteCharacter(char c)
    {
        if (HasSelection)
        {
            ReplaceSelection(c.ToString());
            return;
        }

        var line = Document.Lines[Caret.Line];
        if (Caret.Column >= line.Content.Length)
        {
            InsertText(c.ToString());
            return;
        }

        var range = new TextRange(Caret, new TextPosition(Caret.Line, Caret.Column + 1));
        var oldChar = line.Content[Caret.Column].ToString();
        var op = new ReplaceRangeOperation(range, c.ToString(), oldChar, Caret);
        Execute(op);
    }

    public void InsertNewLine() => InsertText(Environment.NewLine);

    public void DeleteWordLeft()
    {
        if (HasSelection)
        {
            DeleteSelection();
            return;
        }

        var caretBefore = Caret;
        Navigator.MoveWordLeft();
        var wordStart = Caret;

        if (wordStart == caretBefore) return;

        Anchor = caretBefore;
        DeleteSelection();
    }

    public void DeleteWordRight()
    {
        if (HasSelection)
        {
            DeleteSelection();
            return;
        }

        var caretBefore = Caret;
        Navigator.MoveWordRight();
        var wordEnd = Caret;

        if (wordEnd == caretBefore) return;

        Anchor = caretBefore;
        DeleteSelection();
    }

    public void ChangeEncoding(Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        Execute(new ChangeEncodingOperation(Document.Encoding, encoding));
    }
}
