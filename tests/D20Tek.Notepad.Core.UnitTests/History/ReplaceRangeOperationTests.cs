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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(range, "Universe", "World", new TextPosition(0, 6));

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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(range, "Universe", "World", new TextPosition(0, 6));
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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, " ", "", new TextPosition(0, 5));

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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, " ", "", new TextPosition(0, 5));
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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(range, "", " World", new TextPosition(0, 5));

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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(range, "", " World", new TextPosition(0, 5));
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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, "Hi", "Hello", new TextPosition(0, 0));

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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var operation = new ReplaceRangeOperation(range, null!, " World", new TextPosition(0, 5));

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
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, " World", null!, new TextPosition(0, 5));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello", doc.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_InsertNewLine_InsertsLineBreakAndUpdatesCaret()
    {
        // arrange
        var doc = new Doc.Document([new("HelloWorld")]);
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, Environment.NewLine, "", new TextPosition(0, 5));

        // act
        operation.Redo(session);

        // assert
        Assert.HasCount(2, doc.Lines);
        Assert.AreEqual("Hello", doc.Lines[0].Content);
        Assert.AreEqual("World", doc.Lines[1].Content);
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
    }

    [TestMethod]
    public void Undo_AfterInsertNewLine_RestoresSingleLine()
    {
        // arrange
        var doc = new Doc.Document([new("HelloWorld")]);
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));
        var operation = new ReplaceRangeOperation(range, Environment.NewLine, "", new TextPosition(0, 5));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual(1, doc.Lines.Count);
        Assert.AreEqual("HelloWorld", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void Redo_InsertMultiLineText_InsertsAllLinesAndUpdatesCaret()
    {
        // arrange
        var doc = new Doc.Document([new("Start End")]);
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 6));
        var multiLineText = $"Line1{Environment.NewLine}Line2{Environment.NewLine}Line3";
        var operation = new ReplaceRangeOperation(range, multiLineText, "", new TextPosition(0, 6));

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual(3, doc.Lines.Count);
        Assert.AreEqual(new TextPosition(2, 5), session.Caret); // "Line3" has 5 characters
    }

    [TestMethod]
    public void Undo_AfterInsertMultiLineText_RestoresOriginalContent()
    {
        // arrange
        var doc = new Doc.Document([new("Start End")]);
        var session = new EditorSession(doc);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 6));
        var multiLineText = $"Line1{Environment.NewLine}Line2";
        var operation = new ReplaceRangeOperation(range, multiLineText, "", new TextPosition(0, 6));
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual(1, doc.Lines.Count);
        Assert.AreEqual("Start End", doc.Lines[0].Content);
    }
}
