namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorCommandServiceTests
{
    [TestMethod]
    public void SelectLeft_ExtendsSelectionLeft()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        service.SelectLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void SelectRight_ExtendsSelectionRight()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.SelectRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    [TestMethod]
    public void SelectUp_ExtendsSelectionUp()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        service.SelectUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(1, 2), session.Anchor);
    }

    [TestMethod]
    public void SelectDown_ExtendsSelectionDown()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);

        // act
        service.SelectDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    [TestMethod]
    public void SelectPageUp_ExtendsSelectionUpByPageHeight()
    {
        // arrange
        var (service, session) = CreateService(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8"]);
        session.Caret = new TextPosition(7, 3);
        session.Anchor = new TextPosition(7, 3);

        // act
        service.SelectPageUp(5);

        // assert
        Assert.AreEqual(new TextPosition(2, 3), session.Caret);
        Assert.AreEqual(new TextPosition(7, 3), session.Anchor);
    }

    [TestMethod]
    public void SelectPageDown_ExtendsSelectionDownByPageHeight()
    {
        // arrange
        var (service, session) = CreateService(["Line1", "Line2", "Line3", "Line4", "Line5", "Line6", "Line7", "Line8"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        service.SelectPageDown(5);

        // assert
        Assert.AreEqual(new TextPosition(5, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void SelectAll_SelectsEntireDocument()
    {
        // arrange
        var (service, session) = CreateService(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 2);
        session.Anchor = new TextPosition(1, 2);

        // act
        service.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void SelectAll_WithSingleLine_SelectsEntireLine()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);

        // act
        service.SelectAll();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }
}
