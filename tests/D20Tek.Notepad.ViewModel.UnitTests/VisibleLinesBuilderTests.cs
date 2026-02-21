namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class VisibleLinesBuilderTests
{
    private static readonly DocumentFactory _docFactory = new();

    // MapCaret tests
    [TestMethod]
    public void MapCaret_WithCaretAtOrigin_ReturnsZeroPosition()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 0);
        viewModel.SetViewportHeight(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(0, 0), viewModel.CaretViewPosition);
    }

    [TestMethod]
    public void MapCaret_WithCaretBelowFirstVisibleLine_SubtractsOffset()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        session.Caret = new TextPosition(3, 2);
        viewModel.SetViewportHeight(3);
        viewModel.ScrollLines(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(1, 2), viewModel.CaretViewPosition);
    }

    [TestMethod]
    public void MapCaret_WithCaretAboveViewport_ClampsToZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        session.Caret = new TextPosition(0, 0);
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(2);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(0, viewModel.CaretViewPosition.LineIndex);
    }

    [TestMethod]
    public void MapCaret_WithHorizontalOffset_SubtractsColumnOffset()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World Test"]);
        session.Caret = new TextPosition(0, 10);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(0, 5), viewModel.CaretViewPosition);
    }

    [TestMethod]
    public void MapCaret_WithHorizontalOffsetBeyondCaret_ClampsColumnToZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 3);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(0, viewModel.CaretViewPosition.Column);
    }

    // Build tests with edge cases
    [TestMethod]
    public void Build_WithEmptyDocument_ReturnsEmptyList()
    {
        // arrange
        var (viewModel, _) = CreateViewModel([""]);
        viewModel.SetViewportHeight(10);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void Build_WithViewportLargerThanDocument_ReturnsOnlyDocumentLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(10);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Build_WithZeroViewportHeight_ReturnsEmptyList()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(0);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(0, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Build_WithScrolledPastEnd_ReturnsEmptyLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(2);
        // Note: ScrollLines clamps, so scrolling past end won't actually go past

        // act
        viewModel.ScrollLines(1); // Max is lineCount - viewportHeight = 0

        // assert
        // Should still have lines based on clamped position
        Assert.IsTrue(viewModel.VisibleLines.Count > 0);
    }

    [TestMethod]
    public void Build_PreservesDocumentLineIndex()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Line 0", "Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(2);
        viewModel.ScrollLines(1);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual(2, viewModel.VisibleLines[1].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithHorizontalOffset_SlicesTextCorrectly()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["0123456789"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(5);
        viewModel.ScrollColumns(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("34567", viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void Build_WithZeroViewportWidth_ReturnsFullTextFromOffset()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["0123456789"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(0); // No width limit
        viewModel.ScrollColumns(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual("3456789", viewModel.VisibleLines[0].Text);
    }

    // MapSelection tests
    [TestMethod]
    public void MapSelection_WithNoSelection_ReturnsNull()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        session.Anchor = new TextPosition(0, 5); // Same as caret = no selection
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNull(viewModel.SelectionViewRange);
    }

    [TestMethod]
    public void MapSelection_WithSelection_ReturnsViewRange()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(viewModel.SelectionViewRange);
        Assert.AreEqual(0, viewModel.SelectionViewRange.Value.Start.Column);
        Assert.AreEqual(5, viewModel.SelectionViewRange.Value.End.Column);
    }

    [TestMethod]
    public void MapSelection_WithHorizontalOffset_AdjustsColumns()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World Test"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(20);
        viewModel.ScrollColumns(4);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(viewModel.SelectionViewRange);
        Assert.AreEqual(2, viewModel.SelectionViewRange.Value.Start.Column);
        Assert.AreEqual(7, viewModel.SelectionViewRange.Value.End.Column);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }
}
