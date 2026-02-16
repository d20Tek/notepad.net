using D20Tek.Notepad.Core.History;
using D20Tek.Notepad.Core.Primitives;

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

    public void InsertNewLine() => InsertText(Environment.NewLine);
}
