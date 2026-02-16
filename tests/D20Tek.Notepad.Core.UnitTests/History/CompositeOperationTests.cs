using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public class CompositeOperationTests
{
    [TestMethod]
    public void Redo_WithMultipleOperations_ExecutesInForwardOrder()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 5), new TextPosition(0, 5));

        var op1 = new InsertTextOperation(
            new TextPosition(0, 5), " World", string.Empty,
            new TextPosition(0, 5), new TextPosition(0, 11));

        var op2 = new InsertTextOperation(
            new TextPosition(0, 11), "!", string.Empty,
            new TextPosition(0, 11), new TextPosition(0, 12));

        var composite = new CompositeOperation([op1, op2]);

        // act
        composite.Redo(session);

        // assert
        Assert.AreEqual("Hello World!", document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_WithMultipleOperations_ExecutesInReverseOrder()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 5), new TextPosition(0, 5));

        var op1 = new InsertTextOperation(
            new TextPosition(0, 5), " World", string.Empty,
            new TextPosition(0, 5), new TextPosition(0, 11));

        var op2 = new InsertTextOperation(
            new TextPosition(0, 11), "!", string.Empty,
            new TextPosition(0, 11), new TextPosition(0, 12));

        var composite = new CompositeOperation([op1, op2]);
        composite.Redo(session);

        // act
        composite.Undo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_AfterRedo_RestoresOriginalState()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 0), new TextPosition(0, 0));

        var deleteOp = new DeleteRangeOperation(
            new TextRange(new TextPosition(0, 5), new TextPosition(0, 11)),
            " World",
            new TextPosition(0, 11), new TextPosition(0, 5));

        var insertOp = new InsertTextOperation(
            new TextPosition(0, 5), "!", string.Empty,
            new TextPosition(0, 5), new TextPosition(0, 6));

        var composite = new CompositeOperation([deleteOp, insertOp]);
        composite.Redo(session);
        Assert.AreEqual("Hello!", document.Lines[0].Content);

        // act
        composite.Undo(session);

        // assert
        Assert.AreEqual("Hello World", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithEmptyOperations_DoesNotThrow()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 0), new TextPosition(0, 0));

        var composite = new CompositeOperation([]);

        // act & assert
        composite.Redo(session);
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_WithEmptyOperations_DoesNotThrow()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 0), new TextPosition(0, 0));

        var composite = new CompositeOperation([]);

        // act & assert
        composite.Undo(session);
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithSingleOperation_ExecutesOperation()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 5), new TextPosition(0, 5));

        var op = new InsertTextOperation(
            new TextPosition(0, 5), " World", string.Empty,
            new TextPosition(0, 5), new TextPosition(0, 11));

        var composite = new CompositeOperation([op]);

        // act
        composite.Redo(session);

        // assert
        Assert.AreEqual("Hello World", document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_MultipleRedoUndoCycles_MaintainsConsistentState()
    {
        // arrange
        var lines = new[] { new TextLine("Test") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 4), new TextPosition(0, 4));

        var op1 = new InsertTextOperation(
            new TextPosition(0, 4), "123", string.Empty,
            new TextPosition(0, 4), new TextPosition(0, 7));

        var op2 = new InsertTextOperation(
            new TextPosition(0, 7), "456", string.Empty,
            new TextPosition(0, 7), new TextPosition(0, 10));

        var composite = new CompositeOperation([op1, op2]);

        // act & assert
        composite.Redo(session);
        Assert.AreEqual("Test123456", document.Lines[0].Content);

        composite.Undo(session);
        Assert.AreEqual("Test", document.Lines[0].Content);

        composite.Redo(session);
        Assert.AreEqual("Test123456", document.Lines[0].Content);

        composite.Undo(session);
        Assert.AreEqual("Test", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithMixedOperationTypes_ExecutesAllOperations()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var session = new EditorSession(document, new TextPosition(0, 0), new TextPosition(0, 0));

        // Delete " World" then insert " Universe"
        var deleteOp = new DeleteRangeOperation(
            new TextRange(new TextPosition(0, 5), new TextPosition(0, 11)),
            " World",
            new TextPosition(0, 11), new TextPosition(0, 5));

        var insertOp = new InsertTextOperation(
            new TextPosition(0, 5), " Universe", string.Empty,
            new TextPosition(0, 5), new TextPosition(0, 14));

        var composite = new CompositeOperation([deleteOp, insertOp]);

        // act
        composite.Redo(session);

        // assert
        Assert.AreEqual("Hello Universe", document.Lines[0].Content);
    }
}