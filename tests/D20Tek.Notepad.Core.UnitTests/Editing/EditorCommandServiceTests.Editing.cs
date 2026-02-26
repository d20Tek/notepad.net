namespace D20Tek.Notepad.Core.UnitTests.Editing;

public partial class EditorCommandServiceTests
{
    [TestMethod]
    public void TypeCharacter_InsertsCharacterAtCaret()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.TypeCharacter('!');

        // assert
        Assert.AreEqual("Hello!", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertText_InsertsTextAtCaret()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.InsertText(" World");

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void InsertNewLine_SplitsLineAtCaret()
    {
        // arrange
        var (service, session) = CreateService(["HelloWorld"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.InsertNewLine();

        // assert
        Assert.AreEqual(2, session.Document.Lines.Count);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
    }

    [TestMethod]
    public void Backspace_DeletesPreviousCharacter()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.Backspace();

        // assert
        Assert.AreEqual("Hell", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Delete_DeletesNextCharacter()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);

        // act
        service.Delete();

        // assert
        Assert.AreEqual("ello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void ReplaceSelection_ReplacesSelectedText()
    {
        // arrange
        var (service, session) = CreateService(["Hello World"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);

        // act
        service.ReplaceSelection("Universe");

        // assert
        Assert.AreEqual("Hello Universe", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void DeleteSelection_DeletesSelectedText()
    {
        // arrange
        var (service, session) = CreateService(["Hello World"]);
        session.Anchor = new TextPosition(0, 5);
        session.Caret = new TextPosition(0, 11);

        // act
        service.DeleteSelection();

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void CopySelection_ReturnsSelectedText()
    {
        // arrange
        var (service, session) = CreateService(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);

        // act
        var result = service.CopySelection();

        // assert
        Assert.AreEqual("Hello", result);
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void CutSelection_ReturnsSelectedTextAndDeletesIt()
    {
        // arrange
        var (service, session) = CreateService(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);

        // act
        var result = service.CutSelection();

        // assert
        Assert.AreEqual("Hello ", result);
        Assert.AreEqual("World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void CutSelection_WithNoSelection_ReturnsEmptyString()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        var result = service.CutSelection();

        // assert
        Assert.AreEqual(string.Empty, result);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_InsertsTextAtCaret()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.Paste(" World");

        // assert
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_WithSelection_ReplacesSelectedText()
    {
        // arrange
        var (service, session) = CreateService(["Hello World"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);

        // act
        service.Paste("Universe");

        // assert
        Assert.AreEqual("Hello Universe", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_WithEmptyString_DoesNotChangeDocument()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.Paste("");

        // assert
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Paste_WithMultiLineText_InsertsAllLines()
    {
        // arrange
        var (service, session) = CreateService(["Hello"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);

        // act
        service.Paste("\nWorld\nTest");

        // assert
        Assert.AreEqual(3, session.Document.LineCount);
        Assert.AreEqual("Hello", session.Document.Lines[0].Content);
        Assert.AreEqual("World", session.Document.Lines[1].Content);
        Assert.AreEqual("Test", session.Document.Lines[2].Content);
    }
}
