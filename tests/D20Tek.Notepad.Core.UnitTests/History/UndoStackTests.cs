using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public partial class UndoStackTests
{
    [TestMethod]
    public void Push_WithValidOperation_AddsToUndoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);

        // act
        stack.Push(op);
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Push_AfterUndo_ClearsRedoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op1) = CreateTestOperation("Hello", " World");
        var op2 = new InsertTextOperation(new TextPosition(0, 11), "!", string.Empty, new TextPosition(0, 11));

        stack.Push(op1);
        op1.Redo(session);
        stack.Undo(session);

        // act
        stack.Push(op2);
        var caretBefore = session.Caret;
        stack.Redo(session);

        // assert
        Assert.AreEqual(caretBefore, session.Caret);
    }

    [TestMethod]
    public void Undo_WithOperationOnStack_ExecutesUndo()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_WithEmptyStack_DoesNothing()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_MovesOperationToRedoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        stack.Undo(session);

        // act
        stack.Redo(session);

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithOperationOnStack_ExecutesRedo()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        stack.Undo(session);

        // act
        stack.Redo(session);

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithEmptyStack_DoesNothing()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        // act
        stack.Redo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_MovesOperationToUndoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        stack.Undo(session);
        stack.Redo(session);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void BeginGroup_WhenNoGroupActive_StartsGrouping()
    {
        // arrange
        var stack = new UndoStack();

        // act
        stack.BeginGroup();
        stack.EndGroup();

        // assert
    }

    [TestMethod]
    public void EndGroup_WithEmptyGroup_DoesNotPushToStack()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        // act
        stack.BeginGroup();
        stack.EndGroup();
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void EndGroup_WithSingleOperation_PushesOperationDirectly()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");

        // act
        stack.BeginGroup();
        stack.Push(op);
        stack.EndGroup();
        op.Redo(session);
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void EndGroup_WithMultipleOperations_CreatesCompositeOperation()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        var op1 = new InsertTextOperation(new TextPosition(0, 5), " World", string.Empty, new TextPosition(0, 5));
        var op2 = new InsertTextOperation(new TextPosition(0, 11), "!", string.Empty, new TextPosition(0, 11));

        stack.BeginGroup();
        stack.Push(op1);
        stack.Push(op2);
        stack.EndGroup();
        op1.Redo(session);
        op2.Redo(session);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Push_DuringGrouping_AccumulatesOperations()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("Test") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        var op1 = new InsertTextOperation(new TextPosition(0, 4), "1", string.Empty, new TextPosition(0, 4));
        var op2 = new InsertTextOperation(new TextPosition(0, 5), "2", string.Empty, new TextPosition(0, 5));
        var op3 = new InsertTextOperation(new TextPosition(0, 6), "3", string.Empty, new TextPosition(0, 6));

        stack.BeginGroup();
        stack.Push(op1);
        stack.Push(op2);
        stack.Push(op3);
        stack.EndGroup();
        op1.Redo(session);
        op2.Redo(session);
        op3.Redo(session);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual("Test", document.Lines[0].Content);
    }

    [TestMethod]
    public void MultipleUndoRedo_MaintainsCorrectOrder()
    {
        // arrange
        var stack = new UndoStack();
        var lines = new[] { new TextLine("A") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document);

        var op1 = new InsertTextOperation(new TextPosition(0, 1), "B", string.Empty, new TextPosition(0, 1));
        var op2 = new InsertTextOperation(new TextPosition(0, 2), "C", string.Empty, new TextPosition(0, 2));

        stack.Push(op1);
        op1.Redo(session);
        stack.Push(op2);
        op2.Redo(session);

        // act
        stack.Undo(session);
        stack.Undo(session);
        stack.Redo(session);
        stack.Redo(session);

        // assert
        Assert.AreEqual("ABC", document.Lines[0].Content);
    }


    private static (EditorSession session, InsertTextOperation op) CreateTestOperation(
        string initialText,
        string textToInsert)
    {
        var lines = new[] { new TextLine(initialText) };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, initialText.Length);
        var newCaret = new TextPosition(0, initialText.Length + textToInsert.Length);
        var session = new EditorSession(document);

        var op = new InsertTextOperation(position, textToInsert, string.Empty, position);

        return (session, op);
    }

    // CanUndo Tests
    [TestMethod]
    public void CanUndo_WithEmptyStack_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.IsFalse(stack.CanUndo);
    }

    [TestMethod]
    public void CanUndo_WithOperationOnStack_ReturnsTrue()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        stack.Push(op);


        // act & assert
        Assert.IsTrue(stack.CanUndo);
    }

    [TestMethod]
    public void CanUndo_AfterUndoingAllOperations_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session); // Apply the operation first
        stack.Push(op);
        stack.Undo(session);

        // act & assert
        Assert.IsFalse(stack.CanUndo);
    }

    // CanRedo Tests
    [TestMethod]
    public void CanRedo_WithEmptyRedoStack_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.IsFalse(stack.CanRedo);
    }

    [TestMethod]
    public void CanRedo_AfterUndo_ReturnsTrue()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session); // Apply the operation first
        stack.Push(op);
        stack.Undo(session);

        // act & assert
        Assert.IsTrue(stack.CanRedo);
    }

    [TestMethod]
    public void CanRedo_AfterRedoingAllOperations_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session); // Apply the operation first
        stack.Push(op);
        stack.Undo(session);
        stack.Redo(session);

        // act & assert
        Assert.IsFalse(stack.CanRedo);
    }

    [TestMethod]
    public void CanRedo_AfterPushingNewOperation_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op1) = CreateTestOperation("Hello", " World");
        op1.Redo(session); // Apply the operation first
        stack.Push(op1);
        stack.Undo(session);
        Assert.IsTrue(stack.CanRedo);

        // act - new push clears redo stack
        var op2 = new InsertTextOperation(new TextPosition(0, 5), "!", string.Empty, new TextPosition(0, 5));
        stack.Push(op2);

        // assert
        Assert.IsFalse(stack.CanRedo);
    }

    // Clear Tests
    [TestMethod]
    public void Clear_ClearsUndoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        stack.Push(op);
        Assert.IsTrue(stack.CanUndo);

        // act
        stack.Clear();

        // assert
        Assert.IsFalse(stack.CanUndo);
    }

    [TestMethod]
    public void Clear_ClearsRedoStack()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        stack.Undo(session);
        Assert.IsTrue(stack.CanRedo);

        // act
        stack.Clear();

        // assert
        Assert.IsFalse(stack.CanRedo);
    }

    [TestMethod]
    public void Clear_ResetsVersionToZero()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        stack.Push(op);
        Assert.AreEqual(1, stack.Version);

        // act
        stack.Clear();

        // assert
        Assert.AreEqual(0, stack.Version);
    }

    [TestMethod]
    public void Clear_ClearsActiveGroup()
    {
        // arrange
        var stack = new UndoStack();
        var (_, op) = CreateTestOperation("Hello", " World");
        stack.BeginGroup();
        stack.Push(op);
        Assert.IsTrue(stack.HasPendingGroupOperations);

        // act
        stack.Clear();

        // assert
        Assert.IsFalse(stack.HasPendingGroupOperations);

        // Should be able to begin a new group without exception
        stack.BeginGroup();
        stack.EndGroup();
    }

    // Version Tests
    [TestMethod]
    public void Version_InitialValue_IsZero()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.AreEqual(0, stack.Version);
    }

    [TestMethod]
    public void Version_IncrementsOnPush()
    {
        // arrange
        var stack = new UndoStack();
        var (_, op1) = CreateTestOperation("Hello", "!");
        var (_, op2) = CreateTestOperation("Hello", "?");

        // act & assert
        stack.Push(op1);
        Assert.AreEqual(1, stack.Version);

        stack.Push(op2);
        Assert.AreEqual(2, stack.Version);
    }

    [TestMethod]
    public void Version_DecrementsOnUndo()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        Assert.AreEqual(1, stack.Version);

        // act
        stack.Undo(session);

        // assert
        Assert.AreEqual(0, stack.Version);
    }

    [TestMethod]
    public void Version_IncrementsOnRedo()
    {
        // arrange
        var stack = new UndoStack();
        var (session, op) = CreateTestOperation("Hello", " World");
        op.Redo(session);
        stack.Push(op);
        stack.Undo(session);
        Assert.AreEqual(0, stack.Version);

        // act
        stack.Redo(session);

        // assert
        Assert.AreEqual(1, stack.Version);
    }

    // HasPendingGroupOperations Tests
    [TestMethod]
    public void HasPendingGroupOperations_WithNoGroup_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.IsFalse(stack.HasPendingGroupOperations);
    }

    [TestMethod]
    public void HasPendingGroupOperations_WithEmptyGroup_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();
        stack.BeginGroup();

        // act & assert
        Assert.IsFalse(stack.HasPendingGroupOperations);
    }

    [TestMethod]
    public void HasPendingGroupOperations_WithOperationsInGroup_ReturnsTrue()
    {
        // arrange
        var stack = new UndoStack();
        var (_, op) = CreateTestOperation("Hello", "!");
        stack.BeginGroup();
        stack.Push(op);

        // act & assert
        Assert.IsTrue(stack.HasPendingGroupOperations);
    }

    [TestMethod]
    public void HasPendingGroupOperations_AfterEndGroup_ReturnsFalse()
    {
        // arrange
        var stack = new UndoStack();
        var (_, op) = CreateTestOperation("Hello", "!");
        stack.BeginGroup();
        stack.Push(op);
        Assert.IsTrue(stack.HasPendingGroupOperations);

        // act
        stack.EndGroup();

        // assert
        Assert.IsFalse(stack.HasPendingGroupOperations);
    }
}