using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelHorizontalTests
{
    private static readonly DocumentFactory _docFactory = new();
    private readonly EditorNavigationService _nav = new();

    // SetViewportWidth tests
    [TestMethod]
    public void SetViewportWidth_WithValidWidth_RefreshesView()
    {
        // arrange
        var session = CreateSession(["Hello World this is a long line"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);

        // act
        viewModel.SetViewportWidth(10);

        // assert
        Assert.AreEqual("Hello Worl", viewModel.VisibleLines[0].Text);
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
        viewModel.Refresh();

        // assert
        Assert.AreEqual("Hello", viewModel.VisibleLines[0].Text);
    }

    // ScrollColumns tests
    [TestMethod]
    public void ScrollColumns_WithPositiveDelta_ScrollsRightAndSlicesText()
    {
        // arrange
        var session = CreateSession(["Hello World this is a long line"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.ScrollColumns(6);

        // assert
        Assert.AreEqual(6, viewModel.Viewport.HorizontalOffset);
        Assert.AreEqual("World this", viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void ScrollColumns_WithNegativeDelta_ScrollsLeft()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(5);
        viewModel.ScrollColumns(6);

        // act
        viewModel.ScrollColumns(-3);

        // assert
        Assert.AreEqual(3, viewModel.Viewport.HorizontalOffset);
        Assert.AreEqual("lo Wo", viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void ScrollColumns_ClampsToZero()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(5);
        viewModel.ScrollColumns(2);

        // act
        viewModel.ScrollColumns(-10);

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    // EnsureCaretVisible horizontal tests
    [TestMethod]
    public void EnsureCaretVisible_WhenCaretLeftOfViewport_ScrollsLeft()
    {
        // arrange
        var session = CreateSession(["Hello World this is a long line"]);
        session.Caret = new TextPosition(0, 5);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(10);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(5, viewModel.Viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureCaretVisible_WhenCaretRightOfViewport_ScrollsRight()
    {
        // arrange
        var session = CreateSession(["Hello World this is a long line"]);
        session.Caret = new TextPosition(0, 20);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(11, viewModel.Viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureCaretVisible_WhenCaretInHorizontalViewport_DoesNotScrollHorizontally()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 5);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(20);

        // act
        viewModel.EnsureCaretVisible();

        // assert
        Assert.AreEqual(0, viewModel.Viewport.HorizontalOffset);
    }

    // BuildVisibleLines horizontal slicing tests
    [TestMethod]
    public void BuildVisibleLines_WithHorizontalOffset_SlicesTextFromOffset()
    {
        // arrange
        var session = CreateSession(["0123456789ABCDEF"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(5);
        viewModel.ScrollColumns(5);

        // act & assert
        Assert.AreEqual("56789", viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void BuildVisibleLines_WhenOffsetBeyondLineLength_ReturnsEmptyText()
    {
        // arrange
        var session = CreateSession(["Short"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(10);

        // act & assert
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void BuildVisibleLines_WhenSliceExceedsLineLength_ReturnsRemainingText()
    {
        // arrange
        var session = CreateSession(["Hello"]);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(2);

        // act & assert
        Assert.AreEqual("llo", viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void BuildVisibleLines_WithNoWidthAndOffsetBeyondLineLength_ReturnsEmptyText()
    {
        // arrange
        var session = CreateSession(["Short"]);
        session.Caret = new TextPosition(0, 10); // Set caret beyond text to avoid negative column issue
        session.Anchor = new TextPosition(0, 10);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        // Don't set viewport width (stays 0 = no limit)
        viewModel.ScrollColumns(10); // Scroll past the 5-character line

        // act & assert
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }
    
    // MapCaret horizontal tests
    [TestMethod]
    public void CaretViewPosition_WithHorizontalOffset_SubtractsOffset()
    {
        // arrange
        var session = CreateSession(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(new ViewPosition(0, 5), viewModel.CaretViewPosition);
    }

    // MapSelection horizontal tests
    [TestMethod]
    public void SelectionViewRange_WithHorizontalOffset_SubtractsOffset()
    {
        // arrange
        var session = CreateSession(["Hello World Test"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(0, 11);
        var viewModel = new EditorViewModel(session, new(session, _nav));
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(20);
        viewModel.ScrollColumns(3);

        // act
        viewModel.Refresh();

        // assert
        Assert.IsNotNull(viewModel.SelectionViewRange);
        Assert.AreEqual(3, viewModel.SelectionViewRange.Value.Start.Column);
        Assert.AreEqual(8, viewModel.SelectionViewRange.Value.End.Column);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        return new EditorSession(doc);
    }
}