using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class EditorNavigationServiceTests
{
    // Basic caret movement
    [TestMethod]
    public void MoveLeft_DelegatesToCaretMovementHelper()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveRight_DelegatesToCaretMovementHelper()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveUp_DelegatesToCaretMovementHelper()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveDown_DelegatesToCaretMovementHelper()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MoveToLineStart tests
    [TestMethod]
    public void MoveToLineStart_AtMiddleOfLine_MovesToColumnZero()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToLineStart(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToLineStart_OnSecondLine_MovesToStartOfCurrentLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToLineStart(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MoveToLineEnd tests
    [TestMethod]
    public void MoveToLineEnd_AtMiddleOfLine_MovesToEndOfLine()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToLineEnd(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToLineEnd_OnSecondLine_MovesToEndOfCurrentLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 1);
        session.Anchor = new TextPosition(1, 1);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToLineEnd(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MoveToDocumentStart tests
    [TestMethod]
    public void MoveToDocumentStart_FromMiddleOfDocument_MovesToOrigin()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToDocumentStart(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToDocumentStart_FromLastLine_MovesToOrigin()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 5);
        session.Anchor = new TextPosition(1, 5);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToDocumentStart(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // MoveToDocumentEnd tests
    [TestMethod]
    public void MoveToDocumentEnd_FromMiddleOfDocument_MovesToEnd()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToDocumentEnd(session);

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveToDocumentEnd_FromFirstLine_MovesToEndOfLastLine()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        var navigation = new EditorNavigationService();

        // act
        navigation.MoveToDocumentEnd(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    // ExtendLeft tests
    [TestMethod]
    public void ExtendLeft_MovesCaretLeftAndPreservesAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendLeft_WithExistingSelection_ExtendsSelection()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Anchor = new TextPosition(0, 4);
        session.Caret = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendLeft(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 1), session.Caret);
        Assert.AreEqual(new TextPosition(0, 4), session.Anchor);
    }

    // ExtendRight tests
    [TestMethod]
    public void ExtendRight_MovesCaretRightAndPreservesAnchor()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendRight_WithExistingSelection_ExtendsSelection()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 3);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendRight(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
    }

    // ExtendUp tests
    [TestMethod]
    public void ExtendUp_MovesCaretUpAndPreservesAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendUp(session);

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(1, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    // ExtendDown tests
    [TestMethod]
    public void ExtendDown_MovesCaretDownAndPreservesAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        var navigation = new EditorNavigationService();

        // act
        navigation.ExtendDown(session);

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToArray();
        var doc = new Doc.Document(textLines);
        return new EditorSession(doc);
    }
}