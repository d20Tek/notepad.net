namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelTests
{
    private static readonly DocumentFactory _docFactory = new();
    private readonly EditorNavigationService _nav = new();

    [TestMethod]
    public void Constructor_WithValidSession_SetsProperties()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);

        // act
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // assert
        Assert.AreSame(session, viewModel.Session);
        Assert.IsNotNull(viewModel.Viewport);
        Assert.IsNotNull(viewModel.VisibleLines);
    }

    [TestMethod]
    public void Constructor_WithValidSession_InitializesEmptyVisibleLines()
    {
        // arrange
        var session = CreateSession(["Hello"]);

        // act
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // assert
        Assert.IsEmpty(viewModel.VisibleLines);
    }

    // SetViewportHeight tests
    [TestMethod]
    public void SetViewportHeight_WithValidCount_UpdatesViewportAndRefreshes()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

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
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.SetViewportHeight(2);

        // assert
        Assert.AreEqual("Line 1", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("Line 2", viewModel.VisibleLines[1].Text);
    }

    // ScrollLines tests
    [TestMethod]
    public void ScrollLines_WithPositiveDelta_ScrollsDownAndRefreshes()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
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
        var session = CreateSession(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(3);

        // act
        viewModel.ScrollLines(-1);

        // assert
        Assert.AreEqual(2, viewModel.Viewport.FirstVisibleLine);
    }

    // ScrollPages tests
    [TestMethod]
    public void ScrollPages_WithPositiveDelta_ScrollsDownByPages()
    {
        // arrange
        var session = CreateSession(GenerateLines(20));
        var viewModel = new EditorViewModel(session, new(session, _nav));
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
        var session = CreateSession(GenerateLines(20));
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(5);
        viewModel.ScrollPages(3);

        // act
        viewModel.ScrollPages(-1);

        // assert
        Assert.AreEqual(10, viewModel.Viewport.FirstVisibleLine);
    }

    // EnsureCaretVisible tests
    [TestMethod]
    public void EnsureCaretVisible_WhenCaretAboveViewport_ScrollsUp()
    {
        // arrange
        var session = CreateSession(GenerateLines(20));
        session.Caret = new TextPosition(2, 0);
        var viewModel = new EditorViewModel(session, new(session, _nav));
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
        var session = CreateSession(GenerateLines(20));
        session.Caret = new TextPosition(15, 0);
        var viewModel = new EditorViewModel(session, new(session, _nav));
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
        var session = CreateSession(GenerateLines(20));
        session.Caret = new TextPosition(2, 0);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(5);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(0, viewModel.Viewport.FirstVisibleLine);
    }

    // Refresh tests
    [TestMethod]
    public void Refresh_UpdatesVisibleLines()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Refresh_MapsCaretPosition()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        session.Caret = new TextPosition(1, 3);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(1, 3), viewModel.CaretViewPosition);
    }

    [TestMethod]
    public void Refresh_WithNoSelection_SelectionViewRangeIsNull()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNull(viewModel.SelectionViewRange);
    }

    [TestMethod]
    public void Refresh_WithSelection_MapsSelectionViewRange()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3"]);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(1, 4);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(viewModel.SelectionViewRange);
        Assert.AreEqual(new ViewPosition(0, 2), viewModel.SelectionViewRange.Value.Start);
        Assert.AreEqual(new ViewPosition(1, 4), viewModel.SelectionViewRange.Value.End);
    }

    // VisibleLines tests
    [TestMethod]
    public void VisibleLines_WithViewportLargerThanDocument_ReturnsAllLines()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.SetViewportHeight(10);

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void VisibleLines_ContainsCorrectDocumentLineIndex()
    {
        // arrange
        var session = CreateSession(["Line 1", "Line 2", "Line 3", "Line 4"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(1);

        // act & assert
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual(2, viewModel.VisibleLines[1].DocumentLineIndex);
    }

    // CaretViewPosition tests
    [TestMethod]
    public void CaretViewPosition_AfterScroll_MapsToViewCoordinates()
    {
        // arrange
        var session = CreateSession(GenerateLines(10));
        session.Caret = new TextPosition(5, 3);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(5);
        viewModel.ScrollLines(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(2, 3), viewModel.CaretViewPosition);
    }

    // GetLineLength tests
    [TestMethod]
    public void GetLineLength_WithValidLineIndex_ReturnsCorrectLength()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        var length = viewModel.GetLineLength(0);

        // assert
        Assert.AreEqual(5, length);
    }

    [TestMethod]
    public void GetLineLength_WithNegativeIndex_ReturnsZero()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        var length = viewModel.GetLineLength(-1);

        // assert
        Assert.AreEqual(0, length);
    }

    [TestMethod]
    public void GetLineLength_WithIndexExceedingLineCount_ReturnsZero()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        var length = viewModel.GetLineLength(10);

        // assert
        Assert.AreEqual(0, length);
    }

    // SetAnchorToCaret tests
    [TestMethod]
    public void SetAnchorToCaret_SetsAnchorToCurrentCaretPosition()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = new TextPosition(0, 0);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.SetAnchorToCaret();

        // assert
        Assert.AreEqual(new TextPosition(1, 3), session.Anchor);
    }

    // MoveCaretTo tests
    [TestMethod]
    public void MoveCaretTo_WithValidPosition_MovesCaretAndAnchor()
    {
        // arrange
        var session = CreateSession(["Hello", "World", "Test"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

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
        var session = CreateSession(["Hello", "World"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.MoveCaretTo(100, 2);

        // assert
        Assert.AreEqual(1, session.Caret.Line);
    }

    [TestMethod]
    public void MoveCaretTo_WithNegativeLine_ClampsToZero()
    {
        // arrange
        var session = CreateSession(["Hello", "World"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.MoveCaretTo(-5, 2);

        // assert
        Assert.AreEqual(0, session.Caret.Line);
    }

    [TestMethod]
    public void MoveCaretTo_WithColumnExceedingLineLength_ClampsToLineEnd()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.MoveCaretTo(0, 100);

        // assert
        Assert.AreEqual(5, session.Caret.Column);
    }

    [TestMethod]
    public void MoveCaretTo_WithNegativeColumn_ClampsToZero()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));

        // act
        viewModel.MoveCaretTo(0, -10);

        // assert
        Assert.AreEqual(0, session.Caret.Column);
    }

    // SetViewportWidth tests
    [TestMethod]
    public void SetViewportWidth_WithPositiveWidth_SetsWidth()
    {
        // arrange
        var session = CreateSession(["Hello World this is a long line"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(20);

        // assert - verify through visible line truncation or refresh behavior
        Assert.HasCount(1, viewModel.VisibleLines);
    }

    [TestMethod]
    public void SetViewportWidth_WithZeroWidth_ClampsToZero()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(0);

        // assert - should not throw
        Assert.HasCount(1, viewModel.VisibleLines);
    }

    [TestMethod]
    public void SetViewportWidth_WithNegativeWidth_ClampsToZero()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(-10);

        // assert - should not throw
        Assert.HasCount(1, viewModel.VisibleLines);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        return new EditorSession(doc);
    }

    private static string[] GenerateLines(int count) => [.. Enumerable.Range(1, count).Select(i => $"Line {i}")];
}
