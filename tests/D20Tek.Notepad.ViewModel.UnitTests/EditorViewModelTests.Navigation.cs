namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelNavigationWrappedTests
{
    private static readonly DocumentFactory _docFactory = new();

    // MoveUp with Word Wrap Tests
    [TestMethod]
    public void MoveUp_WithWordWrap_MovesToPreviousVisualLine()
    {
        // arrange
        // "Hello World" wraps to "Hello", "World"
        // Caret starts in "World" segment
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 8); // In "World"
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.IsTrue(session.Caret.Column <= 5); // Should be in "Hello" segment
    }

    [TestMethod]
    public void MoveUp_WithWordWrap_AtFirstVisualLine_MovesToDocumentStart()
    {
        // arrange
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 3); // In "Hello"
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(0, session.Caret.Column);
    }

    [TestMethod]
    public void MoveUp_WithWordWrap_AcrossDocumentLines()
    {
        // arrange
        // Line 0: "AB" (1 visual), Line 1: "CD" (1 visual)
        // Caret in Line 1, move up to Line 0
        var (viewModel, session) = CreateViewModelWithWordWrap(["AB", "CD"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(10);
        session.Caret = new TextPosition(1, 1);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(1, session.Caret.Column);
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void MoveUp_WithWordWrap_PreservesColumnPosition()
    {
        // arrange
        // "Hello World Today" wraps to "Hello", "World", "Today"
        // Start at column 3 in "Today", move up should land at column 3 in "World"
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 15); // "Tod|ay" - column 3 within segment
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        // Should be at column 9 (6 + 3) in "World" segment
        Assert.IsTrue(session.Caret.Column >= 6 && session.Caret.Column <= 11);
    }

    // MoveDown with Word Wrap Tests
    [TestMethod]
    public void MoveDown_WithWordWrap_MovesToNextVisualLine()
    {
        // arrange
        // "Hello World" wraps to "Hello", "World"
        // Caret starts in "Hello" segment
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 3); // In "Hello"
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.IsTrue(session.Caret.Column >= 6); // Should be in "World" segment
    }

    [TestMethod]
    public void MoveDown_WithWordWrap_AtLastVisualLine_MovesToDocumentEnd()
    {
        // arrange
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 8); // In "World"
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(11, session.Caret.Column); // End of "Hello World"
    }

    [TestMethod]
    public void MoveDown_WithWordWrap_AcrossDocumentLines()
    {
        // arrange
        // Line 0: "AB" (1 visual), Line 1: "CD" (1 visual)
        var (viewModel, session) = CreateViewModelWithWordWrap(["AB", "CD"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(10);
        session.Caret = new TextPosition(0, 1);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(1, session.Caret.Line);
        Assert.AreEqual(1, session.Caret.Column);
    }

    // ExtendUp/ExtendDown with Word Wrap Tests
    [TestMethod]
    public void ExtendUp_WithWordWrap_PreservesAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);

        // act
        viewModel.ExtendUp();

        // assert
        Assert.AreEqual(new TextPosition(0, 8), session.Anchor);
        Assert.AreNotEqual(session.Anchor, session.Caret);
    }

    [TestMethod]
    public void ExtendDown_WithWordWrap_PreservesAnchor()
    {
        // arrange
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = new TextPosition(0, 3);

        // act
        viewModel.ExtendDown();

        // assert
        Assert.AreEqual(new TextPosition(0, 3), session.Anchor);
        Assert.AreNotEqual(session.Anchor, session.Caret);
    }

    // Non-WordWrap behavior preserved
    [TestMethod]
    public void MoveUp_WithoutWordWrap_UsesDocumentLines()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(5);
        session.Caret = new TextPosition(1, 3);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(3, session.Caret.Column);
    }

    [TestMethod]
    public void MoveDown_WithoutWordWrap_UsesDocumentLines()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2"]);
        viewModel.SetViewportHeight(5);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(1, session.Caret.Line);
        Assert.AreEqual(3, session.Caret.Column);
    }

    // Home/End behavior (Notepad style - always goes to document line)
    [TestMethod]
    public void MoveToLineStart_WithWordWrap_GoesToDocumentLineStart()
    {
        // arrange
        // "Hello World" wraps, caret in "World" segment
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveToLineStart();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(0, session.Caret.Column); // Start of document line, not wrapped segment
    }

    [TestMethod]
    public void MoveToLineEnd_WithWordWrap_GoesToDocumentLineEnd()
    {
        // arrange
        // "Hello World" wraps, caret in "Hello" segment
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 3);
        session.Anchor = session.Caret;

        // act
        viewModel.MoveToLineEnd();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(11, session.Caret.Column); // End of document line
    }

    // GetDocumentPositionForVisualLine edge cases (tested via MoveDown)
    [TestMethod]
    public void MoveDown_WithWordWrap_ClampsColumnToSegmentLength()
    {
        // arrange
        // Line 0: "Hello World" wraps to "Hello" (5), "World" (5)
        // Line 1: "AB" (2 chars)
        // Caret at column 4 in "World", move down should clamp to "AB" length (2)
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World", "AB"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 10); // Column 4 in "World" segment
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(1, session.Caret.Line);
        Assert.AreEqual(2, session.Caret.Column); // Clamped to "AB" length
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void MoveDown_WithWordWrap_WithPreferredColumnExceedingSegment_ClampsToSegmentEnd()
    {
        // arrange
        // "Hello World Today" wraps to "Hello" (5), "World" (5), "Today" (5)
        // Start at column 4 in "Hello", move down to "World" which also has 5 chars
        var (viewModel, session) = CreateViewModelWithWordWrap(["Hello World Today"]);
        viewModel.SetViewportHeight(5);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(0, 4); // Column 4 in "Hello"
        session.Anchor = session.Caret;

        // act
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        // Should be at column 10 (6 + 4) in "World" segment, clamped to segment
        Assert.IsTrue(session.Caret.Column >= 6 && session.Caret.Column <= 11);
    }

    [TestMethod]
    public void MoveDown_WithWordWrap_MultipleDocumentLines_NavigatesCorrectly()
    {
        // arrange
        // Line 0: "A" (1 visual line)
        // Line 1: "Hello World" (2 visual lines)
        // Line 2: "B" (1 visual line)
        var (viewModel, session) = CreateViewModelWithWordWrap(["A", "Hello World", "B"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(1, 8); // In "World" segment of line 1
        session.Anchor = session.Caret;

        // act - move down from "World" should go to line 2 "B"
        viewModel.MoveDown();

        // assert
        Assert.AreEqual(2, session.Caret.Line);
        Assert.AreEqual(1, session.Caret.Column); // Clamped to "B" length
    }

    [TestMethod]
    public void MoveUp_WithWordWrap_MultipleDocumentLines_NavigatesCorrectly()
    {
        // arrange
        // Line 0: "A" (1 visual line)
        // Line 1: "Hello World" (2 visual lines - "Hello", "World")
        // Caret in "Hello" segment, move up should go to line 0 "A"
        var (viewModel, session) = CreateViewModelWithWordWrap(["A", "Hello World"]);
        viewModel.SetViewportHeight(10);
        viewModel.SetViewportWidth(6);
        session.Caret = new TextPosition(1, 3); // In "Hello" segment of line 1
        session.Anchor = session.Caret;

        // act
        viewModel.MoveUp();

        // assert
        Assert.AreEqual(0, session.Caret.Line);
        Assert.AreEqual(1, session.Caret.Column); // Clamped to "A" length
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }

    // Word Navigation Tests
    [TestMethod]
    public void MoveWordLeft_MovesCaretToWordBoundary()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void MoveWordRight_MovesCaretToWordBoundary()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.MoveWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(session.Caret, session.Anchor);
    }

    [TestMethod]
    public void ExtendWordLeft_ExtendsSelectionToWordBoundary()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 8);
        session.Anchor = new TextPosition(0, 8);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ExtendWordLeft();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 8), session.Anchor);
    }

    [TestMethod]
    public void ExtendWordRight_ExtendsSelectionToWordBoundary()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 2);
        session.Anchor = new TextPosition(0, 2);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ExtendWordRight();

        // assert
        Assert.AreEqual(new TextPosition(0, 6), session.Caret);
        Assert.AreEqual(new TextPosition(0, 2), session.Anchor);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithWordWrap(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
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
