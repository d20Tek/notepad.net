using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
public class DeleteRangeOperationTests
{
    [TestMethod]
    public void Redo_WithTextRange_DeletesTextFromDocument()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var oldText = " World";
        var oldCaret = new TextPosition(0, 11);
        var newCaret = new TextPosition(0, 5);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new DeleteRangeOperation(range, oldText, oldCaret, newCaret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
        Assert.AreEqual(newCaret, session.Caret);
        Assert.AreEqual(newCaret, session.Anchor);
    }

    [TestMethod]
    public void Undo_AfterDelete_RestoresDeletedText()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var oldText = " World";
        var oldCaret = new TextPosition(0, 11);
        var newCaret = new TextPosition(0, 5);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new DeleteRangeOperation(range, oldText, oldCaret, newCaret);
        operation.Redo(session);

        // act
        operation.Undo(session);

        // assert
        Assert.AreEqual("Hello World", document.Lines[0].Content);
        Assert.AreEqual(oldCaret, session.Caret);
        Assert.AreEqual(oldCaret, session.Anchor);
    }

    [TestMethod]
    public void Redo_DeleteFromBeginning_RemovesTextAtStart()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 6));
        var oldText = "Hello ";
        var oldCaret = new TextPosition(0, 6);
        var newCaret = new TextPosition(0, 0);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new DeleteRangeOperation(range, oldText, oldCaret, newCaret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("World", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_DeleteEntireLine_LeavesEmptyContent()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));
        var oldText = "Hello";
        var oldCaret = new TextPosition(0, 5);
        var newCaret = new TextPosition(0, 0);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new DeleteRangeOperation(range, oldText, oldCaret, newCaret);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual(string.Empty, document.Lines[0].Content);
    }

    [TestMethod]
    public void Undo_MultipleRedoUndo_MaintainsConsistentState()
    {
        // arrange
        var lines = new[] { new TextLine("Hello World") };
        var document = new Doc.Document(lines);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));
        var oldText = " World";
        var oldCaret = new TextPosition(0, 11);
        var newCaret = new TextPosition(0, 5);
        var session = new EditorSession(document, oldCaret, oldCaret);

        var operation = new DeleteRangeOperation(range, oldText, oldCaret, newCaret);

        // act & assert - multiple redo/undo cycles
        operation.Redo(session);
        Assert.AreEqual("Hello", document.Lines[0].Content);

        operation.Undo(session);
        Assert.AreEqual("Hello World", document.Lines[0].Content);

        operation.Redo(session);
        Assert.AreEqual("Hello", document.Lines[0].Content);

        operation.Undo(session);
        Assert.AreEqual("Hello World", document.Lines[0].Content);
    }

    [TestMethod]
    public void Redo_WithEmptyRange_DoesNotModifyContent()
    {
        // arrange
        var lines = new[] { new TextLine("Hello") };
        var document = new Doc.Document(lines);
        var position = new TextPosition(0, 3);
        var range = new TextRange(position, position);
        var session = new EditorSession(document, position, position);

        var operation = new DeleteRangeOperation(range, string.Empty, position, position);

        // act
        operation.Redo(session);

        // assert
        Assert.AreEqual("Hello", document.Lines[0].Content);
    }
}