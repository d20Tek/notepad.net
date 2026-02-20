namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorCommandServiceTests
{
    [TestMethod]
    public void MoveLeft_MovesCaretLeft()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        service.MoveLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
    }

    [TestMethod]
    public void MoveRight_MovesCaretRight()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.MoveRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
    }

    [TestMethod]
    public void MoveUp_MovesCaretUp()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        service.MoveUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
    }

    [TestMethod]
    public void MoveDown_MovesCaretDown()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.MoveDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
    }

    [TestMethod]
    public void PageUp_MovesCaretUpByPageHeight()
    {
        // arrange
        var (service, session) = CreateService(["Line1", "Line2", "Line3", "Line4", "Line5"]);
        session.Caret = new TextPosition(4, 2);
        session.Anchor = new TextPosition(4, 2);

        // act
        service.PageUp(3);

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
    }

    [TestMethod]
    public void PageDown_MovesCaretDownByPageHeight()
    {
        // arrange
        var (service, session) = CreateService(["Line1", "Line2", "Line3", "Line4", "Line5"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.PageDown(3);

        // assert
        Assert.AreEqual(new TextPosition(3, 2), session.Caret);
    }

    [TestMethod]
    public void MoveToLineStart_MovesCaretToStartOfLine()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        service.MoveToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveToLineEnd_MovesCaretToEndOfLine()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.MoveToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void MoveToDocumentStart_MovesCaretToOrigin()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);

        // act
        service.MoveToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveToDocumentEnd_MovesCaretToEnd()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        service.MoveToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(1, 5), session.Caret);
    }
}
