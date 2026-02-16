using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.History;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.Editing;

public sealed partial class EditorSession(IDocument document)
{
    public IDocument Document { get; } = document ?? throw new ArgumentNullException(nameof(document));

    public TextPosition Caret { get; set; } = new TextPosition(0, 0);

    public TextPosition Anchor { get; set; } = new TextPosition(0, 0);

    public UndoStack UndoStack { get; } = new();

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
}
