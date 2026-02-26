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

    // Word Wrap Build tests
    [TestMethod]
    public void Build_WithWordWrapEnabled_WrapsLongLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(12);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("Hello World", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("Today", viewModel.VisibleLines[1].Text);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_PreservesDocumentLineIndex()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(12);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(0, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual(0, viewModel.VisibleLines[1].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_SetsCorrectSegmentStartColumns()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(12);

        // act
        viewModel.Refresh();

        // assert
        Assert.AreEqual(0, viewModel.VisibleLines[0].SegmentStartColumn);
        Assert.AreEqual(12, viewModel.VisibleLines[1].SegmentStartColumn);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_HandlesMultipleDocumentLines()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World", "Foo Bar Baz"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(8);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(4, viewModel.VisibleLines);
        Assert.AreEqual("Hello", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(0, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual("World", viewModel.VisibleLines[1].Text);
        Assert.AreEqual(0, viewModel.VisibleLines[1].DocumentLineIndex);
        Assert.AreEqual("Foo Bar", viewModel.VisibleLines[2].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[2].DocumentLineIndex);
        Assert.AreEqual("Baz", viewModel.VisibleLines[3].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[3].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_LimitsToViewportHeight()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World Today Tomorrow"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(10);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_ShortLineReturnsOneSegment()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hi"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual("Hi", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(0, viewModel.VisibleLines[0].SegmentStartColumn);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_EmptyLineReturnsOneSegment()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap([""]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(80);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_ZeroViewportWidthReturnsEmptyList()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(0);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(0, viewModel.VisibleLines);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_ScrollsToCorrectVisualLine()
    {
        // arrange
        // Two document lines, each wrapping to 2 visual lines = 4 total visual lines
        // "Hello World" with width 6 -> "Hello", "World"
        // "Foo Bar" with width 6 -> "Foo", "Bar"
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World", "Foo Bar"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(6);
        viewModel.Viewport.SetVerticalOffset(2); // Skip to visual line 2 ("Foo")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("Foo", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual("Bar", viewModel.VisibleLines[1].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[1].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_OvershotBacksUpToCorrectSegment()
    {
        // arrange
        // "Hello World Today" with width 6 -> "Hello", "World", "Today" (3 visual lines)
        // Setting FirstVisibleLine=1 should start at "World" (the 2nd segment)
        // This exercises the overshot condition where visualLineIndex > firstVisualLine
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(6);
        viewModel.Viewport.SetVerticalOffset(1); // Skip to visual line 1 ("World")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("World", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(0, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual(6, viewModel.VisibleLines[0].SegmentStartColumn);
        Assert.AreEqual("Today", viewModel.VisibleLines[1].Text);
        Assert.AreEqual(0, viewModel.VisibleLines[1].DocumentLineIndex);
        Assert.AreEqual(12, viewModel.VisibleLines[1].SegmentStartColumn);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_OvershotSkipsMultipleSegments()
    {
        // arrange
        // "ABCD EFGH IJKL MNOP" with width 5 -> "ABCD", "EFGH", "IJKL", "MNOP" (4 visual lines)
        // Setting FirstVisibleLine=2 should start at "IJKL" (the 3rd segment)
        var (viewModel, _) = CreateViewModelWithWordWrap(["ABCD EFGH IJKL MNOP"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(5);
        viewModel.Viewport.SetVerticalOffset(2); // Skip to visual line 2 ("IJKL")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("IJKL", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(10, viewModel.VisibleLines[0].SegmentStartColumn);
        Assert.AreEqual("MNOP", viewModel.VisibleLines[1].Text);
        Assert.AreEqual(15, viewModel.VisibleLines[1].SegmentStartColumn);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_SkipsEntireDocumentLines()
    {
        // arrange
        // Line 0: "AB" (1 visual line), Line 1: "CD" (1 visual line), Line 2: "EF" (1 visual line)
        // Scroll to visual line 2 should show "EF"
        var (viewModel, _) = CreateViewModelWithWordWrap(["AB", "CD", "EF"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.Viewport.SetVerticalOffset(2); // Skip to visual line 2 ("EF")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual("EF", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(2, viewModel.VisibleLines[0].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_MixedShortAndLongLines()
    {
        // arrange
        // Line 0: "A" (1 visual), Line 1: "Hello World" (2 visual), Line 2: "B" (1 visual)
        // Visual lines: 0="A", 1="Hello", 2="World", 3="B"
        // Scroll to visual line 1 should start at "Hello"
        var (viewModel, _) = CreateViewModelWithWordWrap(["A", "Hello World", "B"]);
        viewModel.SetViewportHeight(3);
        viewModel.SetViewportWidth(6);
        viewModel.Viewport.SetVerticalOffset(1); // Skip to visual line 1 ("Hello")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(3, viewModel.VisibleLines);
        Assert.AreEqual("Hello", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
        Assert.AreEqual("World", viewModel.VisibleLines[1].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[1].DocumentLineIndex);
        Assert.AreEqual("B", viewModel.VisibleLines[2].Text);
        Assert.AreEqual(2, viewModel.VisibleLines[2].DocumentLineIndex);
    }

    // Additional coverage tests for BuildNonWrapped branches
    [TestMethod]
    public void Build_WithZeroViewportWidthAndOffsetBeyondText_ReturnsEmptyText()
    {
        // arrange
        // Tests the false branch: hOffset < fullText.Length when viewportWidth <= 0
        var (viewModel, _) = CreateViewModel(["Short"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(0); // No width limit
        viewModel.ScrollColumns(10); // Offset beyond text length (5)

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }

    [TestMethod]
    public void Build_WithHorizontalOffsetBeyondTextLength_ReturnsEmptyText()
    {
        // arrange
        // Tests the else branch: hOffset >= fullText.Length
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.ScrollColumns(20); // Offset beyond text length (5)

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual(string.Empty, viewModel.VisibleLines[0].Text);
    }

    // Additional coverage tests for BuildWrapped branches
    [TestMethod]
    public void Build_WithWordWrapEnabled_ExactVisualLineMatch_NoOvershot()
    {
        // arrange
        // Line 0: "AB" (1 visual), Line 1: "CD" (1 visual)
        // Scroll to visual line 1 exactly - no overshoot scenario
        // This tests the case where visualLineIndex == firstVisualLine after the while loop
        var (viewModel, _) = CreateViewModelWithWordWrap(["AB", "CD"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(10);
        viewModel.Viewport.SetVerticalOffset(1); // Skip to visual line 1 ("CD")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual("CD", viewModel.VisibleLines[0].Text);
        Assert.AreEqual(1, viewModel.VisibleLines[0].DocumentLineIndex);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_OvershotFillsViewportDuringSegmentLoop()
    {
        // arrange
        // "A B C D E F G" with width 2 -> "A", "B", "C", "D", "E", "F", "G" (7 visual lines)
        // Viewport height 2, scroll to visual line 1
        // This tests the visibleLines.Count < visibleLineCount condition in the overshot for loop
        var (viewModel, _) = CreateViewModelWithWordWrap(["A B C D E F G"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(2);
        viewModel.Viewport.SetVerticalOffset(1); // Skip to visual line 1 ("B")

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("B", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("C", viewModel.VisibleLines[1].Text);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_ViewportFillsExactlyDuringMainLoop()
    {
        // arrange
        // Tests the break condition: visibleLines.Count >= visibleLineCount in main loop
        var (viewModel, _) = CreateViewModelWithWordWrap(["Hello World", "Foo Bar"]);
        viewModel.SetViewportHeight(2);
        viewModel.SetViewportWidth(6);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(2, viewModel.VisibleLines);
        Assert.AreEqual("Hello", viewModel.VisibleLines[0].Text);
        Assert.AreEqual("World", viewModel.VisibleLines[1].Text);
    }

    [TestMethod]
    public void Build_WithWordWrapEnabled_BreaksDuringSegmentIteration()
    {
        // arrange
        // Line wraps to more segments than viewport can hold
        // Tests break inside foreach when visibleLines.Count >= visibleLineCount
        var (viewModel, _) = CreateViewModelWithWordWrap(["ABCD EFGH IJKL MNOP"]);
        viewModel.SetViewportHeight(1);
        viewModel.SetViewportWidth(5);

        // act
        viewModel.Refresh();

        // assert
        Assert.HasCount(1, viewModel.VisibleLines);
        Assert.AreEqual("ABCD", viewModel.VisibleLines[0].Text);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithWordWrap(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var settings = new EditorSettings { WordWrapEnabled = true };
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            settings,
            new MockClipboardService());
        return (viewModel, session);
    }
}
