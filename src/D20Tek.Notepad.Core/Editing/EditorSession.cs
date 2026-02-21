using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.History;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed partial class EditorSession
{
    public EditorSession(IDocument document)
    {
        Document = document ?? throw new ArgumentNullException(nameof(document));
        Navigator = new CaretNavigator(this);
    }

    public IDocument Document { get; private set; }

    public TextPosition Caret { get; set; } = new TextPosition(0, 0);

    public TextPosition Anchor { get; set; } = new TextPosition(0, 0);

    public UndoStack UndoStack { get; } = new();

    public CaretNavigator Navigator { get; }

    /// <summary>
    /// Clamps a position to valid document bounds.
    /// Line is clamped to [0, LineCount-1], column is clamped to [0, LineLength].
    /// </summary>
    public TextPosition ClampToDocument(int line, int column)
    {
        line = Math.Clamp(line, 0, Document.Lines.Count - 1);
        column = Math.Clamp(column, 0, Document.Lines[line].Content.Length);
        return new TextPosition(line, column);
    }

    /// <summary>
    /// Gets the length of a line, or 0 if the line index is out of bounds.
    /// </summary>
    public int GetLineLength(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= Document.Lines.Count)
            return 0;

        return Document.Lines[lineIndex].Content.Length;
    }

    internal TextPosition ApplyReplaceRange(TextRange range, string replacement)
    {
        var newCaret = RangeEditingService.ReplaceRange(Document, range, replacement);
        Caret = newCaret;
        Anchor = newCaret;

        return newCaret;
    }

    public void Execute(IUndoableOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        UndoStack.Push(operation);
        operation.Redo(this);
    }

    public void Undo() => UndoStack.Undo(this);

    public void Redo() => UndoStack.Redo(this);

    public void ReplaceDocument(IDocument newDocument)
    {
        Document = newDocument;
        Caret = new TextPosition(0, 0);
        ClearSelection();

        // Notify listeners (ViewModel) that the document changed
        //DocumentChanged?.Invoke(this, EventArgs.Empty);
    }
}
