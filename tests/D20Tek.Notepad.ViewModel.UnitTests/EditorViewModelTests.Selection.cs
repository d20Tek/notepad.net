using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Editing;
using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelSelectionTests
{
    private static readonly DocumentFactory _docFactory = new();

    // GetSelectionSegmentForLine - No selection tests
    [TestMethod]
    public void GetSelectionSegmentForLine_WithNoSelection_ReturnsNull()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "World"]);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3); // No selection
        viewModel.SetViewportHeight(2);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNull(result);
    }

    // GetSelectionSegmentForLine - Line outside selection tests
    [TestMethod]
    public void GetSelectionSegmentForLine_WhenLineBeforeSelection_ReturnsNull()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        session.Anchor = new TextPosition(1, 0);
        session.Caret = new TextPosition(2, 3);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetSelectionSegmentForLine_WhenLineAfterSelection_ReturnsNull()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(1, 3);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(2);

        // assert
        Assert.IsNull(result);
    }

    // GetSelectionSegmentForLine - Single-line selection tests
    [TestMethod]
    public void GetSelectionSegmentForLine_WithSingleLineSelection_ReturnsCorrectSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 2);
        session.Caret = new TextPosition(0, 7);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Value.StartColumn);
        Assert.AreEqual(7, result.Value.EndColumn);
    }

    [TestMethod]
    public void GetSelectionSegmentForLine_WithSingleLineSelectionAtStart_ReturnsCorrectSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Value.StartColumn);
        Assert.AreEqual(5, result.Value.EndColumn);
    }

    // GetSelectionSegmentForLine - Multi-line selection first line tests
    [TestMethod]
    public void GetSelectionSegmentForLine_OnFirstLineOfMultiLineSelection_ReturnsSegmentToEndOfLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World", "Second Line", "Third Line"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(2, 5);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(6, result.Value.StartColumn);
        Assert.AreEqual(11, result.Value.EndColumn); // "Hello World".Length = 11
    }

    // GetSelectionSegmentForLine - Multi-line selection last line tests
    [TestMethod]
    public void GetSelectionSegmentForLine_OnLastLineOfMultiLineSelection_ReturnsSegmentFromStart()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World", "Second Line", "Third Line"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(2, 5);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(2);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Value.StartColumn);
        Assert.AreEqual(5, result.Value.EndColumn);
    }

    // GetSelectionSegmentForLine - Multi-line selection middle line tests
    [TestMethod]
    public void GetSelectionSegmentForLine_OnMiddleLineOfMultiLineSelection_ReturnsFullLineSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World", "Second Line", "Third Line"]);
        session.Anchor = new TextPosition(0, 6);
        session.Caret = new TextPosition(2, 5);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(1);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Value.StartColumn);
        Assert.AreEqual(11, result.Value.EndColumn); // "Second Line".Length = 11
    }

    [TestMethod]
    public void GetSelectionSegmentForLine_OnMultipleMiddleLines_ReturnsFullLineSegments()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3", "Line 4", "Line 5"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(4, 6);
        viewModel.SetViewportHeight(5);
        viewModel.Refresh();

        // act
        var result1 = viewModel.GetSelectionSegmentForLine(1);
        var result2 = viewModel.GetSelectionSegmentForLine(2);
        var result3 = viewModel.GetSelectionSegmentForLine(3);

        // assert
        Assert.IsNotNull(result1);
        Assert.AreEqual(0, result1.Value.StartColumn);
        Assert.AreEqual(6, result1.Value.EndColumn);

        Assert.IsNotNull(result2);
        Assert.AreEqual(0, result2.Value.StartColumn);
        Assert.AreEqual(6, result2.Value.EndColumn);

        Assert.IsNotNull(result3);
        Assert.AreEqual(0, result3.Value.StartColumn);
        Assert.AreEqual(6, result3.Value.EndColumn);
    }

    // GetSelectionSegmentForLine - Backward selection tests
    [TestMethod]
    public void GetSelectionSegmentForLine_WithBackwardSelection_ReturnsCorrectSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Anchor = new TextPosition(0, 8);
        session.Caret = new TextPosition(0, 2); // Backward selection
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Value.StartColumn);
        Assert.AreEqual(8, result.Value.EndColumn);
    }

    // GetSelectionSegmentForLine - Edge case tests
    [TestMethod]
    public void GetSelectionSegmentForLine_WithEmptyLineInSelection_ReturnsZeroLengthSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello", "", "World"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(2, 5);
        viewModel.SetViewportHeight(3);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(1);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Value.StartColumn);
        Assert.AreEqual(0, result.Value.EndColumn);
    }

    [TestMethod]
    public void GetSelectionSegmentForLine_WithSelectionAtLineEnd_ReturnsCorrectSegment()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello"]);
        session.Anchor = new TextPosition(0, 3);
        session.Caret = new TextPosition(0, 5);
        viewModel.SetViewportHeight(1);
        viewModel.Refresh();

        // act
        var result = viewModel.GetSelectionSegmentForLine(0);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Value.StartColumn);
        Assert.AreEqual(5, result.Value.EndColumn);
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
}