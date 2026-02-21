namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelViewportTests
{
    private static readonly DocumentFactory _docFactory = new();

    // SetViewportHeight Tests
    [TestMethod]
    public void SetViewportHeight_WithValidCount_UpdatesViewportAndRefreshes()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);

        // act
        viewModel.SetViewportHeight(3);

        // assert
        Assert.AreEqual(3, viewModel.Viewport.VisibleLineCount);
        Assert.HasCount(3, viewModel.VisibleLines);
    }

    [TestMethod]
    public void SetViewportHeight_BuildsCorrectVisibleLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);

        // act
        viewModel.SetViewportHeight(2);

        // assert
        Assert.AreEqual("Line 1", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("Line 2", viewModel.VisibleLines[1].Text);
    }

    // SetViewportWidth Tests
    [TestMethod]
    public void SetViewportWidth_WithPositiveWidth_SetsWidth()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World this is a long line"]);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(20);

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual(20, viewModel.ViewportWidth);
    }

    [TestMethod]
    public void SetViewportWidth_WithZeroWidth_ClampsToZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(0);

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
    }

    [TestMethod]
    public void SetViewportWidth_WithNegativeWidth_ClampsToZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(-10);

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
    }

    // ScrollLines Tests
    [TestMethod]
    public void ScrollLines_WithPositiveDelta_ScrollsDownAndRefreshes()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.ScrollLines(2);

        // assert
        Assert.AreEqual(2, viewModel.Viewport.FirstVisibleLine);
        Assert.AreEqual("Line 3", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("Line 4", viewModel.VisibleLines[1].Text);
    }

    [TestMethod]
    public void ScrollLines_WithNegativeDelta_ScrollsUp()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(3);

        // act
        viewModel.ScrollLines(-1);

        // assert
        Assert.AreEqual(2, viewModel.Viewport.FirstVisibleLine);
    }

    // ScrollPages Tests
    [TestMethod]
    public void ScrollPages_WithPositiveDelta_ScrollsDownByPages()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(GenerateLines(20));
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ScrollPages(2);

        // assert
        Assert.AreEqual(10, viewModel.Viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollPages_WithNegativeDelta_ScrollsUpByPages()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(GenerateLines(20));
        viewModel.SetViewportHeight(5);
        viewModel.ScrollPages(3);

        // act
        viewModel.ScrollPages(-1);

        // assert
        Assert.AreEqual(10, viewModel.Viewport.FirstVisibleLine);
    }

    // EnsureCaretVisible Tests
    [TestMethod]
    public void EnsureCaretVisible_WhenCaretAboveViewport_ScrollsUp()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(2, 0);
        viewModel.SetViewportHeight(5);
        viewModel.ScrollLines(10);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(2, viewModel.Viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureCaretVisible_WhenCaretBelowViewport_ScrollsDown()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(15, 0);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(11, viewModel.Viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureCaretVisible_WhenCaretAlreadyVisible_DoesNotScroll()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(GenerateLines(20));
        session.Caret = new TextPosition(2, 0);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(0, viewModel.Viewport.FirstVisibleLine);
    }

    // VisibleLines Tests
    [TestMethod]
    public void VisibleLines_WithViewportLargerThanDocument_ReturnsAllLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2"]);

        // act
        viewModel.SetViewportHeight(10);

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void VisibleLines_ContainsCorrectDocumentLineIndex()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4"]);
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(1);

        // act & assert
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual(2, viewModel.VisibleLines[1].DocumentLineIndex);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }

    private static string[] GenerateLines(int count) =>
        [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}
