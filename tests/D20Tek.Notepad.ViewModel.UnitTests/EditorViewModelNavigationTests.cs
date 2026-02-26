namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelNavigationTests
{
    private static readonly DocumentFactory _docFactory = new();

    // Movement Tests
    [TestMethod]
    public void MoveLeft_MovesCaretAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.MoveLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
    }

    [TestMethod]
    public void MoveRight_MovesCaretAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.MoveRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
    }

    [TestMethod]
    public void MoveUp_MovesCaretAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
    }

    [TestMethod]
    public void MoveDown_MovesCaretAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
    }

    [TestMethod]
    public void MovePageUp_MovesCaretUpByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(15, 3);
        session.Anchor = new TextPosition(15, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MovePageUp();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
    }

    [TestMethod]
    public void MovePageDown_MovesCaretDownByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(5, 3);
        session.Anchor = new TextPosition(5, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MovePageDown();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
    }

    [TestMethod]
    public void MoveToLineStart_MovesCaretToColumnZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.MoveToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveToLineEnd_MovesCaretToEndOfLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.MoveToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
    }

    [TestMethod]
    public void MoveToDocumentStart_MovesCaretToOrigin()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(2, 3);
        session.Anchor = new TextPosition(2, 3);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.MoveToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveToDocumentEnd_MovesCaretToEndOfDocument()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.MoveToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
    }

    // Selection Extension Tests
    [TestMethod]
    public void ExtendLeft_ExtendsSelectionLeft()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.ExtendLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 2), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendRight_ExtendsSelectionRight()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.ExtendRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
        Assert.IsTrue(session.HasSelection);
    }

    [TestMethod]
    public void ExtendUp_ExtendsSelectionUp()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.ExtendUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendDown_ExtendsSelectionDown()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.ExtendDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendPageUp_ExtendsSelectionUpByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(15, 3);
        session.Anchor = new TextPosition(15, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ExtendPageUp();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
        Assert.AreEqual(new TextPosition(15, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendPageDown_ExtendsSelectionDownByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(5, 3);
        session.Anchor = new TextPosition(5, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ExtendPageDown();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
        Assert.AreEqual(new TextPosition(5, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendToLineStart_ExtendsSelectionToColumnZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 6);
        session.Anchor = new TextPosition(0, 6);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.ExtendToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
    }

    [TestMethod]
    public void ExtendToLineEnd_ExtendsSelectionToEndOfLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.ExtendToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendToDocumentStart_ExtendsSelectionToOrigin()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.ExtendToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    [TestMethod]
    public void ExtendToDocumentEnd_ExtendsSelectionToEndOfDocument()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(1, 3);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.ExtendToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    // GetLineLength Tests
    [TestMethod]
    public void GetLineLength_WithValidLineIndex_ReturnsCorrectLength()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);

        // act
        var length = viewModel.GetLineLength(0);

        // assert
        Assert.AreEqual(5, length);
    }

    [TestMethod]
    public void GetLineLength_WithNegativeIndex_ReturnsZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act
        var length = viewModel.GetLineLength(-1);

        // assert
        Assert.AreEqual(0, length);
    }

    [TestMethod]
    public void GetLineLength_WithIndexExceedingLineCount_ReturnsZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello", "World"]);

        // act
        var length = viewModel.GetLineLength(10);

        // assert
        Assert.AreEqual(0, length);
    }

    // SetAnchorToCaret Tests
    [TestMethod]
    public void SetAnchorToCaret_SetsAnchorToCurrentCaretPosition()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(0, 0);

        // act
        viewModel.SetAnchorToCaret();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    // MoveCaretTo Tests
    [TestMethod]
    public void MoveCaretTo_WithValidPosition_MovesCaretAndAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);

        // act
        viewModel.MoveCaretTo(1, 3);

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Caret);
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    [TestMethod]
    public void MoveCaretTo_WithLineExceedingDocument_ClampsToLastLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);

        // act
        viewModel.MoveCaretTo(100, 2);

        // assert
        Assert.AreEqual(1, session.Caret.Line);
    }

    [TestMethod]
    public void MoveCaretTo_WithNegativeLine_ClampsToZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);

        // act
        viewModel.MoveCaretTo(-5, 2);

        // assert
        Assert.AreEqual(0, session.Caret.Line);
    }

    [TestMethod]
    public void MoveCaretTo_WithColumnExceedingLineLength_ClampsToLineEnd()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);

        // act
        viewModel.MoveCaretTo(0, 100);

        // assert
        Assert.AreEqual(5, session.Caret.Column);
    }

    [TestMethod]
    public void MoveCaretTo_WithNegativeColumn_ClampsToZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);

        // act
        viewModel.MoveCaretTo(0, -10);

        // assert
        Assert.AreEqual(0, session.Caret.Column);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var commandService = new EditorCommandService(session);
        var viewModel = new EditorViewModel(session, commandService, EditorSettings.Default, new MockClipboardService());
        return (viewModel, session);
    }

    private static string[] GenerateLines(int count) =>
        [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}
