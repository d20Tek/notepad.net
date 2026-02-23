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

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithWordWrap(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var settings = new EditorSettings { WordWrapEnabled = true };
        var viewModel = new EditorViewModel(session, new EditorCommandService(session), settings);
        return (viewModel, session);
    }

    private static string[] GenerateLines(int count) =>
        [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];

    // Word Wrap Viewport Tests
    [TestMethod]
    public void GetTotalVisualLineCount_WithoutWordWrap_ReturnsDocumentLineCount()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World", "Foo Bar"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(6);

        // act
        var result = viewModel.GetTotalVisualLineCount();

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetTotalVisualLineCount_WithWordWrap_ReturnsTotalVisualLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World", "Foo"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(6);

        // act
        var result = viewModel.GetTotalVisualLineCount();

        // assert
        Assert.AreEqual(3, result); // 2 from first line + 1 from second
    }

    [TestMethod]
    public void ScrollColumns_WithWordWrapEnabled_DoesNotScroll()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World is a long line"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.ScrollColumns(5);

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ScrollColumns_WithoutWordWrap_Scrolls()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World is a long line"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.ScrollColumns(5);

        // assert
        Assert.AreEqual(5, viewModel.Viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ScrollLines_WithWordWrap_UsesVisualLineCount()
    {
        // arrange
        // "Hello World" wraps to 2 visual lines, "Foo" is 1 visual line = 3 total
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World", "Foo"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(6);

        // act
        viewModel.ScrollLines(1);

        // assert
        Assert.AreEqual(1, viewModel.Viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureCaretVisible_WithWordWrap_ScrollsToVisualLine()
    {
        // arrange
        // "Hello World Today" wraps to 3 visual lines
        // Caret at column 10 (in "Today") should be visual line 2
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(6);
        viewModel.Viewport.SetVerticalOffset(0);
        session.Caret = new TextPosition(0, 14); // Position in "Today"

        // act
        viewModel.EnsureCaretVisible();

        // assert
        // Should have scrolled to make visual line 2 visible
        Assert.IsTrue(viewModel.Viewport.FirstVisibleLine >= 1);
    }

    [TestMethod]
    public void EnsureCaretVisible_WithWordWrap_DoesNotScrollHorizontally()
    {
        // arrange
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 14);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }
}
