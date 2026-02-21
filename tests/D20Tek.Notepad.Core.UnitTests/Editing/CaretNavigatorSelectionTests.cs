using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class CaretNavigatorSelectionTests
{
    // ============================================================
    // ExtendLeft Tests
    // ============================================================

    [TestMethod]
    public void ExtendLeft_AtMiddleOfLine_MovesCaretLeftWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.ExtendLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendLeft_AtStartOfLine_MovesToEndOfPreviousLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);

        // act
        session.Navigator.ExtendLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
        Assert.AreEqual(new TextPosition(1, 0), session.Anchor);
    }

    // ============================================================
    // ExtendRight Tests
    // ============================================================

    [TestMethod]
    public void ExtendRight_AtMiddleOfLine_MovesCaretRightWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        session.Navigator.ExtendRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendRight_AtEndOfLine_MovesToStartOfNextLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        session.Navigator.ExtendRight();

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 5), session.Anchor);
    }

    // ============================================================
    // ExtendUp Tests
    // ============================================================

    [TestMethod]
    public void ExtendUp_AtMiddleLine_MovesToPreviousLineWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        session.Navigator.ExtendUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendUp_WhenPreviousLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hi", "Hello"]);
        session.Caret = new TextPosition(1, 4);
        session.Anchor = new TextPosition(1, 4);

        // act
        session.Navigator.ExtendUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(1, 4), session.Anchor);
    }

    // ============================================================
    // ExtendDown Tests
    // ============================================================

    [TestMethod]
    public void ExtendDown_AtMiddleLine_MovesToNextLineWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.ExtendDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendDown_WhenNextLineShorter_ClampsColumn()
    {
        // arrange
        var session = CreateSession(["Hello", "Hi"]);
        session.Caret = new TextPosition(0, 4);
        session.Anchor = new TextPosition(0, 4);

        // act
        session.Navigator.ExtendDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 4), session.Anchor);
    }

    // ============================================================
    // ExtendPageUp/Down Tests
    // ============================================================

    [TestMethod]
    public void ExtendPageUp_AtMiddleOfDocument_MovesUpByPageHeightWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(7, 3);
        session.Anchor = new TextPosition(7, 3);

        // act
        session.Navigator.ExtendPageUp(5);

        // assert
        Assert.AreEqual(new TextPosition(2, 3), session.Caret);
        Assert.AreEqual(new TextPosition(7, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendPageDown_AtMiddleOfDocument_MovesDownByPageHeightWithoutChangingAnchor()
    {
        // arrange
        var session = CreateSession(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8", "Line9", "Line10"]);
        session.Caret = new TextPosition(2, 3);
        session.Anchor = new TextPosition(2, 3);

        // act
        session.Navigator.ExtendPageDown(5);

        // assert
        Assert.AreEqual(new TextPosition(7, 3), session.Caret);
        Assert.AreEqual(new TextPosition(2, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ============================================================
    // ExtendToLineStart/End Tests
    // ============================================================

    [TestMethod]
    public void ExtendToLineStart_ExtendsSelectionToColumnZero()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);

        // act
        session.Navigator.ExtendToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendToLineEnd_ExtendsSelectionToEndOfLine()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        session.Navigator.ExtendToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ============================================================
    // ExtendToDocumentStart/End Tests
    // ============================================================

    [TestMethod]
    public void ExtendToDocumentStart_ExtendsSelectionToOrigin()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        session.Navigator.ExtendToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendToDocumentEnd_ExtendsSelectionToEndOfLastLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        session.Navigator.ExtendToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}
