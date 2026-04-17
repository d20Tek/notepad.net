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

    public void DuplicateLine()
    {
        if (HasSelection)
        {
            var range = GetSelectionRange();
            var selectedText = GetTextInRange(range);
            var endOfSelection = range.End;

            Caret = endOfSelection;
            Anchor = endOfSelection;

            var textToInsert = Environment.NewLine + selectedText;
            var op = new InsertTextOperation(endOfSelection, textToInsert, string.Empty, endOfSelection);
            Execute(op);
        }
        else
        {
            var lineContent = Document.Lines[Caret.Line].Content;
            var endOfLine = new TextPosition(Caret.Line, lineContent.Length);
            var textToInsert = Environment.NewLine + lineContent;

            var savedCaret = Caret;
            var op = new InsertTextOperation(endOfLine, textToInsert, string.Empty, savedCaret);
            Execute(op);

            Caret = new TextPosition(savedCaret.Line + 1, savedCaret.Column);
            Anchor = Caret;
        }
    }

    public void MoveLineUp()
    {
        if (Caret.Line == 0) return;

        int line = Caret.Line;
        int savedColumn = Caret.Column;
        var currentContent = Document.Lines[line].Content;
        var aboveContent = Document.Lines[line - 1].Content;

        var ops = new List<IUndoableOperation>
        {
            new ReplaceRangeOperation(
                new TextRange(new TextPosition(line - 1, 0), new TextPosition(line - 1, aboveContent.Length)),
                currentContent, aboveContent, Caret),
            new ReplaceRangeOperation(
                new TextRange(new TextPosition(line, 0), new TextPosition(line, currentContent.Length)),
                aboveContent, currentContent, Caret)
        };

        var composite = new CompositeOperation(ops);
        Execute(composite);

        Caret = new TextPosition(line - 1, Math.Min(savedColumn, currentContent.Length));
        Anchor = Caret;
    }

    public void MoveLineDown()
    {
        if (Caret.Line >= Document.Lines.Count - 1) return;

        int line = Caret.Line;
        int savedColumn = Caret.Column;
        var currentContent = Document.Lines[line].Content;
        var belowContent = Document.Lines[line + 1].Content;

        var ops = new List<IUndoableOperation>
        {
            new ReplaceRangeOperation(
                new TextRange(new TextPosition(line, 0), new TextPosition(line, currentContent.Length)),
                belowContent, currentContent, Caret),
            new ReplaceRangeOperation(
                new TextRange(new TextPosition(line + 1, 0), new TextPosition(line + 1, belowContent.Length)),
                currentContent, belowContent, Caret)
        };

        var composite = new CompositeOperation(ops);
        Execute(composite);

        Caret = new TextPosition(line + 1, Math.Min(savedColumn, currentContent.Length));
        Anchor = Caret;
    }

    public void IndentLines(string indent)
    {
        ArgumentNullException.ThrowIfNull(indent);

        var range = HasSelection ? GetSelectionRange() : new TextRange(Caret, Caret);
        int startLine = range.Start.Line;
        int endLine = range.End.Line;

        var ops = new List<IUndoableOperation>();
        for (int i = startLine; i <= endLine; i++)
        {
            var pos = new TextPosition(i, 0);
            ops.Add(new InsertTextOperation(pos, indent, string.Empty, Caret));
        }

        var composite = new CompositeOperation(ops);
        Execute(composite);

        Caret = new TextPosition(Caret.Line, Caret.Column + indent.Length);
        if (HasSelection || startLine != endLine)
        {
            Anchor = new TextPosition(startLine, 0);
            Caret = new TextPosition(endLine, Document.Lines[endLine].Content.Length);
        }
        else
        {
            Anchor = Caret;
        }
    }

    public void OutdentLines(string indent)
    {
        ArgumentNullException.ThrowIfNull(indent);

        var range = HasSelection ? GetSelectionRange() : new TextRange(Caret, Caret);
        int startLine = range.Start.Line;
        int endLine = range.End.Line;

        var ops = new List<IUndoableOperation>();
        for (int i = startLine; i <= endLine; i++)
        {
            var lineContent = Document.Lines[i].Content;
            if (lineContent.StartsWith(indent))
            {
                var deleteRange = new TextRange(new TextPosition(i, 0), new TextPosition(i, indent.Length));
                ops.Add(new DeleteRangeOperation(deleteRange, indent, Caret));
            }
            else
            {
                int whitespaceCount = 0;
                while (whitespaceCount < lineContent.Length && whitespaceCount < indent.Length &&
                       char.IsWhiteSpace(lineContent[whitespaceCount]))
                {
                    whitespaceCount++;
                }

                if (whitespaceCount > 0)
                {
                    var deleteRange = new TextRange(
                        new TextPosition(i, 0), new TextPosition(i, whitespaceCount));
                    ops.Add(new DeleteRangeOperation(deleteRange, lineContent[..whitespaceCount], Caret));
                }
            }
        }

        if (ops.Count == 0) return;

        var composite = new CompositeOperation(ops);
        Execute(composite);

        if (HasSelection || startLine != endLine)
        {
            Anchor = new TextPosition(startLine, 0);
            Caret = new TextPosition(endLine, Document.Lines[endLine].Content.Length);
        }
        else
        {
            int col = Math.Max(0, Caret.Column);
            Caret = new TextPosition(Caret.Line, Math.Min(col, Document.Lines[Caret.Line].Content.Length));
            Anchor = Caret;
        }
    }

    public void TrimTrailingWhitespace()
    {
        var ops = new List<IUndoableOperation>();
        for (int i = 0; i < Document.Lines.Count; i++)
        {
            var content = Document.Lines[i].Content;
            var trimmed = content.TrimEnd();
            if (trimmed.Length < content.Length)
            {
                var range = new TextRange(
                    new TextPosition(i, trimmed.Length), new TextPosition(i, content.Length));
                ops.Add(new DeleteRangeOperation(range, content[trimmed.Length..], Caret));
            }
        }

        if (ops.Count == 0) return;

        var savedCaret = Caret;
        var composite = new CompositeOperation(ops);
        Execute(composite);

        int clampedCol = Math.Min(savedCaret.Column, Document.Lines[savedCaret.Line].Content.Length);
        Caret = new TextPosition(savedCaret.Line, clampedCol);
        Anchor = Caret;
    }
}
