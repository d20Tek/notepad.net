using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class CaretNavigatorMovementTests
{
    // ============================================================
    // MoveLeft Tests
    // ============================================================

    [TestMethod]
    public void MoveLeft_AtMiddleOfLine_MovesCaretOneColumnLeft()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.MoveLeft();

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
        session.Navigator.MoveLeft();

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
        session.Navigator.MoveLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    // ============================================================
    // MoveRight Tests
    // ============================================================

    [TestMethod]
    public void MoveRight_AtMiddleOfLine_MovesCaretOneColumnRight()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.Navigator.MoveRight();

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
        session.Navigator.MoveRight();

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
        session.Navigator.MoveRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    // ============================================================
    // MoveUp Tests
    // ============================================================

    [TestMethod]
    public void MoveUp_AtMiddleLine_MovesToPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        session.Navigator.MoveUp();

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
        session.Navigator.MoveUp();

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
        session.Navigator.MoveUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ============================================================
    // MoveDown Tests
    // ============================================================

    [TestMethod]
    public void MoveDown_AtMiddleLine_MovesToNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.MoveDown();

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
        session.Navigator.MoveDown();

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
        session.Navigator.MoveDown();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ============================================================
    // MovePageUp Tests
    // ============================================================

    [TestMethod]
    public void MovePageUp_AtMiddleOfDocument_MovesUpByPageHeight()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(7, 3);
        session.Anchor = new TextPosition(7, 3);

        // act
        session.Navigator.MovePageUp(5);

        // assert
        Assert.AreEqual(new TextPosition(2, 3), session.Caret);
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
        session.Navigator.MovePageUp(5);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ============================================================
    // MovePageDown Tests
    // ============================================================

    [TestMethod]
    public void MovePageDown_AtMiddleOfDocument_MovesDownByPageHeight()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(2, 3);
        session.Anchor = new TextPosition(2, 3);

        // act
        session.Navigator.MovePageDown(5);

        // assert
        Assert.AreEqual(new TextPosition(7, 3), session.Caret);
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
        session.Navigator.MovePageDown(5);

        // assert
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ============================================================
    // MoveToLineStart/End Tests
    // ============================================================

    [TestMethod]
    public void MoveToLineStart_MovesCaretToColumnZero()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);

        // act
        session.Navigator.MoveToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToLineEnd_MovesCaretToEndOfLine()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.MoveToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ============================================================
    // MoveToDocumentStart/End Tests
    // ============================================================

    [TestMethod]
    public void MoveToDocumentStart_MovesToOrigin()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        session.Navigator.MoveToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToDocumentEnd_MovesToEndOfLastLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.MoveToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
