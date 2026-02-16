using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public class InsertTextOperationTests
{
    [TestMethod]
    public void Undo_WithInsertedText_RestoresCaretPosition()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 5);
        var oldCaret = new TextPosition(0, 5);
        var newCaret = new TextPosition(0, 6);
        var session = new EditorSession(document, newCaret, newCaret);

        var operation = new InsertTextOperation(position, " ", string.Empty, oldCaret, newCaret);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual(oldCaret, session.Caret);
        Assert.AreEqual(oldCaret, session.Anchor);
    }

    [TestMethod]
    public void Redo_WithInsertedText_InsertsTextAndUpdatesCaret()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 5);
        var oldCaret = new TextPosition(0, 5);
        var newCaret = new TextPosition(0, 11);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new InsertTextOperation(position, " World", string.Empty, oldCaret, newCaret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello World", document.Lines[0].Content);
        Assert.AreEqual(newCaret, session.Caret);
        Assert.AreEqual(newCaret, session.Anchor);
    }

    [TestMethod]
    public void Undo_AfterRedo_RestoresOriginalContent()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 5);
        var oldCaret = new TextPosition(0, 5);
        var newCaret = new TextPosition(0, 11);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new InsertTextOperation(position, " World", string.Empty, oldCaret, newCaret);

        // act
        operation.Redo(session);
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
        Assert.AreEqual(oldCaret, session.Caret);
    }

    [TestMethod]
    public void Redo_AtBeginningOfLine_InsertsTextAtStart()
    {
        // arrange
        var lines = new[] { new TextLine("World") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 0);
        var oldCaret = new TextPosition(0, 0);
        var newCaret = new TextPosition(0, 6);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new InsertTextOperation(position, "Hello ", string.Empty, oldCaret, newCaret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello World", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithEmptyText_DoesNotModifyContent()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 5);
        var caret = new TextPosition(0, 5);
        var session = new EditorSession(document, caret, caret);

        var operation = new InsertTextOperation(position, string.Empty, string.Empty, caret, caret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }
}