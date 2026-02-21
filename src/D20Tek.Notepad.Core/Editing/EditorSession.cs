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

    public TextPosition ClampToDocument(int line, int column)
    {
        line = Math.Clamp(line, 0, Document.Lines.Count - 1);
        column = Math.Clamp(column, 0, Document.Lines[line].Content.Length);
        return new TextPosition(line, column);
    }

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
