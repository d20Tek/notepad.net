using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class SelectionMovementHelperTests
{
    // MoveLeft tests
    [TestMethod]
    public void MoveLeft_AtMiddleOfLine_MovesCaretLeftWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        SelectionMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void MoveLeft_AtStartOfLine_MovesToEndOfPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);

        // act
        SelectionMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(new TextPosition(1, 0), session.Anchor);
    }

    [TestMethod]
    public void MoveLeft_AtDocumentStart_DoesNotMove()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        SelectionMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
    }

    // MoveRight tests
    [TestMethod]
    public void MoveRight_AtMiddleOfLine_MovesCaretRightWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        SelectionMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    [TestMethod]
    public void MoveRight_AtEndOfLine_MovesToStartOfNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        SelectionMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
    }

    [TestMethod]
    public void MoveRight_AtDocumentEnd_DoesNotMove()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        SelectionMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
    }

    // MoveUp tests
    [TestMethod]
    public void MoveUp_AtMiddleLine_MovesToPreviousLineWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        SelectionMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    [TestMethod]
    public void MoveUp_WhenPreviousLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hi", "Hello"]);
        session.Caret = new TextPosition(1, 4);
        session.Anchor = new TextPosition(1, 4);

        // act
        SelectionMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(1, 4), session.Anchor);
    }

    [TestMethod]
    public void MoveUp_AtFirstLine_MovesToLineStart()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        SelectionMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void MoveUp_WhenPreviousLineEmpty_MovesToColumnZero()
    {
        // arrange
        var session = CreateSession(["", "Hello"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        SelectionMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    // MoveDown tests
    [TestMethod]
    public void MoveDown_AtMiddleLine_MovesToNextLineWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        SelectionMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void MoveDown_WhenNextLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hello", "Hi"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);

        // act
        SelectionMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 4), session.Anchor);
    }

    [TestMethod]
    public void MoveDown_AtLastLine_MovesToLineEnd()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        SelectionMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    [TestMethod]
    public void MoveDown_WhenNextLineEmpty_MovesToColumnZero()
    {
        // arrange
        var session = CreateSession(["Hello", ""]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        SelectionMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    // Selection extension tests
    [TestMethod]
    public void MoveRight_WithExistingSelection_ExtendsSelection()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        SelectionMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void MoveLeft_WithExistingSelection_ExtendsSelectionBackward()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 3);

        // act
        SelectionMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
