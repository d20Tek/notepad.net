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
