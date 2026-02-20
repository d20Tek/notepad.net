namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class NavigatorTests
{
    private static readonly DocumentFactory _docFactory = new();

    // MoveCaretLeft tests
    [TestMethod]
    public void MoveCaretLeft_MovesCaretLeftAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.MoveCaretLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 4), session.Caret);
    }

    [TestMethod]
    public void MoveCaretLeft_AtStartOfLine_MovesToEndOfPreviousLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(1, 0);
        session.Anchor = new TextPosition(1, 0);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Navigator.MoveCaretLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    // MoveCaretRight tests
    [TestMethod]
    public void MoveCaretRight_MovesCaretRightAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.MoveCaretRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
    }

    [TestMethod]
    public void MoveCaretRight_AtEndOfLine_MovesToStartOfNextLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Navigator.MoveCaretRight();

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
    }

    // MoveCaretUp tests
    [TestMethod]
    public void MoveCaretUp_MovesCaretUpAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(2, 2);
        session.Anchor = new TextPosition(2, 2);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.MoveCaretUp();

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
    }

    [TestMethod]
    public void MoveCaretUp_WhenAboveViewport_ScrollsViewport()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(10));
        session.Caret = new TextPosition(3, 0);
        session.Anchor = new TextPosition(3, 0);
        viewModel.SetViewportHeight(3);
        viewModel.ScrollLines(3);

        // act
        viewModel.Navigator.MoveCaretUp();

        // assert
        Assert.AreEqual(new TextPosition(2, 0), session.Caret);
        Assert.AreEqual(2, viewModel.Viewport.FirstVisibleLine);
    }

    // MoveCaretDown tests
    [TestMethod]
    public void MoveCaretDown_MovesCaretDownAndEnsuresVisible()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.MoveCaretDown();

        // assert
        Assert.AreEqual(new TextPosition(1, 2), session.Caret);
    }

    [TestMethod]
    public void MoveCaretDown_WhenBelowViewport_ScrollsViewport()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(10));
        session.Caret = new TextPosition(2, 0);
        session.Anchor = new TextPosition(2, 0);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.MoveCaretDown();

        // assert
        Assert.AreEqual(new TextPosition(3, 0), session.Caret);
        Assert.AreEqual(1, viewModel.Viewport.FirstVisibleLine);
    }

    // MovePageUp tests
    [TestMethod]
    public void MovePageUp_MovesCaretUpByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(15, 3);
        session.Anchor = new TextPosition(15, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Navigator.MovePageUp();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
    }

    // MovePageDown tests
    [TestMethod]
    public void MovePageDown_MovesCaretDownByVisibleLineCount()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(5, 3);
        session.Anchor = new TextPosition(5, 3);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Navigator.MovePageDown();

        // assert
        Assert.AreEqual(new TextPosition(10, 3), session.Caret);
    }

    // MoveToLineStart tests
    [TestMethod]
    public void MoveToLineStart_MovesCaretToColumnZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.MoveToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void MoveToLineStart_WithHorizontalScroll_ScrollsToStart()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World this is a long line"]);
        session.Caret = new TextPosition(0, 20);
        session.Anchor = new TextPosition(0, 20);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(10);

        // act
        viewModel.Navigator.MoveToLineStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    // MoveToLineEnd tests
    [TestMethod]
    public void MoveToLineEnd_MovesCaretToEndOfLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Navigator.MoveToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
    }

    [TestMethod]
    public void MoveToLineEnd_WithHorizontalScroll_ScrollsToShowCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World this is a long line"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.Navigator.MoveToLineEnd();

        // assert
        Assert.AreEqual(new TextPosition(0, 31), session.Caret);
        Assert.IsTrue(viewModel.Viewport.HorizontalOffset > 0);
    }

    // MoveToDocumentStart tests
    [TestMethod]
    public void MoveToDocumentStart_MovesCaretToOrigin()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(10));
        session.Caret = new TextPosition(5, 3);
        session.Anchor = new TextPosition(5, 3);
        viewModel.SetViewportHeight(5);
        viewModel.ScrollLines(3);

        // act
        viewModel.Navigator.MoveToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(0, viewModel.Viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void MoveToDocumentStart_ResetsHorizontalScroll()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World this is a long line"]);
        session.Caret = new TextPosition(0, 20);
        session.Anchor = new TextPosition(0, 20);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(10);

        // act
        viewModel.Navigator.MoveToDocumentStart();

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    // MoveToDocumentEnd tests
    [TestMethod]
    public void MoveToDocumentEnd_MovesCaretToEndOfLastLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World", "Test"]);
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Navigator.MoveToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(2, 4), session.Caret);
    }

    [TestMethod]
    public void MoveToDocumentEnd_ScrollsViewportToShowLastLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(0, 0);
        session.Anchor = new TextPosition(0, 0);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.Navigator.MoveToDocumentEnd();

        // assert
        Assert.AreEqual(new TextPosition(19, 7), session.Caret);
        Assert.IsTrue(viewModel.Viewport.FirstVisibleLine >= 15);
    }

    // Helper methods
    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var navigation = new EditorNavigationService();
        var commandService = new EditorCommandService(session, navigation);
        var viewModel = new EditorViewModel(session, commandService);
        return (viewModel, session);
    }

    private static string[] GenerateLines(int count) =>
        [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}