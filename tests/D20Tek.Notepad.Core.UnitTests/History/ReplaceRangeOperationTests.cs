using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public class ReplaceRangeOperationTests
{
    [TestMethod]
    public void Redo_WithReplacementText_AppliesReplacementAndUpdatesCaret()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 0), new TextPosition(0, 0));
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(
            range,
            newText: "Universe",
            oldText: "World",
            oldCaret: new TextPosition(0, 6),
            newCaret: new TextPosition(0, 14));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello Universe", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 14), session.Caret);
        Assert.AreEqual(new TextPosition(0, 14), session.Anchor);
    }

    [TestMethod]
    public void Undo_AfterRedo_RestoresOriginalTextAndCaret()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 0), new TextPosition(0, 0));
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(
            range,
            newText: "Universe",
            oldText: "World",
            oldCaret: new TextPosition(0, 6),
            newCaret: new TextPosition(0, 14));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello World", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
    }

    [TestMethod]
    public void Redo_InsertText_InsertsAtPosition()
    {
        // arrange
        var doc = new Doc.Document([new("HelloWorld")]);
        var session = new EditorSession(doc, new TextPosition(0, 5), new TextPosition(0, 5));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(
            range,
            newText: " ",
            oldText: "",
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 6));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello World", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
    }

    [TestMethod]
    public void Undo_AfterInsert_RemovesInsertedText()
    {
        // arrange
        var doc = new Doc.Document([new("HelloWorld")]);
        var session = new EditorSession(doc, new TextPosition(0, 5), new TextPosition(0, 5));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(
            range,
            newText: " ",
            oldText: "",
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 6));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("HelloWorld", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void Redo_DeleteText_RemovesSelectedText()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 5), new TextPosition(0, 5));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(
            range,
            newText: "",
            oldText: " World",
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 5));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void Undo_AfterDelete_RestoresDeletedText()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 5), new TextPosition(0, 5));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(
            range,
            newText: "",
            oldText: " World",
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 5));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello World", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void RedoUndo_MultipleRoundTrips_RestoresCorrectState()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 0), new TextPosition(0, 0));
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(
            range,
            newText: "Hi",
            oldText: "Hello",
            oldCaret: new TextPosition(0, 0),
            newCaret: new TextPosition(0, 2));

        // act & assert - first redo
        operation.Redo(session);
        Assert.AreEqual("Hi World", doc.Lines[0].Content);

        // act & assert - first undo
        operation.Undo(session);
        Assert.AreEqual("Hello World", doc.Lines[0].Content);

        // act & assert - second redo
        operation.Redo(session);
        Assert.AreEqual("Hi World", doc.Lines[0].Content);

        // act & assert - second undo
        operation.Undo(session);
        Assert.AreEqual("Hello World", doc.Lines[0].Content);
    }

    [TestMethod]
    public void Constructor_NullNewText_TreatsAsEmptyString()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var session = new EditorSession(doc, new TextPosition(0, 0), new TextPosition(0, 0));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(
            range,
            newText: null!,
            oldText: " World",
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 5));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello", doc.Lines[0].Content);
    }

    [TestMethod]
    public void Constructor_NullOldText_TreatsAsEmptyString()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);
        var session = new EditorSession(doc, new TextPosition(0, 5), new TextPosition(0, 5));
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(
            range,
            newText: " World",
            oldText: null!,
            oldCaret: new TextPosition(0, 5),
            newCaret: new TextPosition(0, 11));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello", doc.Lines[0].Content);
    }
}
