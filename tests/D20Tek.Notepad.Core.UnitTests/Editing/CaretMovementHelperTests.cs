using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class CaretMovementHelperTests
{
    // MoveLeft tests
    [TestMethod]
    public void MoveLeft_AtMiddleOfLine_MovesCaretOneColumnLeft()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveLeft_AtStartOfLine_MovesToEndOfPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);

        // act
        CaretMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveLeft_AtDocumentStart_DoesNotMove()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        CaretMovementHelper.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    // MoveRight tests
    [TestMethod]
    public void MoveRight_AtMiddleOfLine_MovesCaretOneColumnRight()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        CaretMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveRight_AtEndOfLine_MovesToStartOfNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        CaretMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveRight_AtDocumentEnd_DoesNotMove()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        CaretMovementHelper.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    // MoveUp tests
    [TestMethod]
    public void MoveUp_AtMiddleLine_MovesToPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        CaretMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveUp_WhenPreviousLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hi", "Hello"]);
        session.Caret = new TextPosition(1, 4);
        session.Anchor = new TextPosition(1, 4);

        // act
        CaretMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
    }

    [TestMethod]
    public void MoveUp_AtFirstLine_MovesToLineStart()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MoveDown tests
    [TestMethod]
    public void MoveDown_AtMiddleLine_MovesToNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveDown_WhenNextLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hello", "Hi"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);

        // act
        CaretMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
    }

    [TestMethod]
    public void MoveDown_AtLastLine_MovesToLineEnd()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        CaretMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveDown_WhenNextLineEmpty_MovesToColumnZero()
    {
        // arrange
        var session = CreateSession(["Hello", ""]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
    }

    [TestMethod]
    public void MoveUp_WhenPreviousLineEmpty_MovesToColumnZero()
    {
        // arrange
        var session = CreateSession(["", "Hello"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        CaretMovementHelper.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    // MovePageUp tests
    [TestMethod]
    public void MovePageUp_AtMiddleOfDocument_MovesUpByPageHeight()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(7, 3);
        session.Anchor = new TextPosition(7, 3);

        // act
        CaretMovementHelper.MovePageUp(session, 5);

        // assert
        Assert.AreEqual(new TextPosition(2, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageUp_WhenTargetLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hi", "Line2", "Line3", "Line4", "Hello"]);
        session.Caret = new TextPosition(4, 4);
        session.Anchor = new TextPosition(4, 4);

        // act
        CaretMovementHelper.MovePageUp(session, 4);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageUp_WhenPageHeightExceedsLines_ClampsToFirstLine()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3"]);
        session.Caret = new TextPosition(2, 3);
        session.Anchor = new TextPosition(2, 3);

        // act
        CaretMovementHelper.MovePageUp(session, 10);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageUp_AtFirstLine_MovesToLineStart()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MovePageUp(session, 5);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MovePageDown tests
    [TestMethod]
    public void MovePageDown_AtMiddleOfDocument_MovesDownByPageHeight()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(2, 3);
        session.Anchor = new TextPosition(2, 3);

        // act
        CaretMovementHelper.MovePageDown(session, 5);

        // assert
        Assert.AreEqual(new TextPosition(7, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageDown_WhenTargetLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hello", "Line2", "Line3", "Line4", "Hi"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);

        // act
        CaretMovementHelper.MovePageDown(session, 4);

        // assert
        Assert.AreEqual(new TextPosition(4, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageDown_WhenPageHeightExceedsLines_ClampsToLastLine()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        CaretMovementHelper.MovePageDown(session, 10);

        // assert
        Assert.AreEqual(new TextPosition(2, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MovePageDown_AtLastLine_MovesToLineEnd()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        CaretMovementHelper.MovePageDown(session, 5);

        // assert
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
