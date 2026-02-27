using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelSearchTests
{
    private static readonly DocumentFactory _docFactory = new();

    // FindNext Tests
    [TestMethod]
    public void FindNext_WithMatch_SelectsMatch()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindNext("World");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(new TextPosition(0, 6), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 11), session.Caret);
    }

    [TestMethod]
    public void FindNext_WithNoMatch_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindNext("xyz");

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindNext_StoresLastSearchTerm()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.FindNext("World");

        // assert
        Assert.AreEqual("World", viewModel.LastSearchTerm);
        Assert.IsTrue(viewModel.HasLastSearch);
    }

    [TestMethod]
    public void FindNext_WithEmptyTerm_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindNext("");

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindNext_WithNullTerm_ThrowsArgumentNullException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => viewModel.FindNext(null!));
    }

    // FindNextFromCurrent Tests
    [TestMethod]
    public void FindNextFromCurrent_UsesLastSearchTerm()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        viewModel.SetViewportHeight(5);
        viewModel.FindNext("test");
        session.Caret = new TextPosition(0, 4);
        session.Anchor = session.Caret;

        // act
        var result = viewModel.FindNextFromCurrent();

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(new TextPosition(0, 9), session.Anchor);
    }

    [TestMethod]
    public void FindNextFromCurrent_WithNoLastSearch_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        var result = viewModel.FindNextFromCurrent();

        // assert
        Assert.IsFalse(result);
    }

    // FindPrevious Tests
    [TestMethod]
    public void FindPrevious_WithMatch_SelectsMatch()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 11);
        session.Anchor = session.Caret;
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindPrevious("Hello");


        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 5), session.Caret);
    }

    [TestMethod]
    public void FindPrevious_WithNoMatch_ReturnsFalse()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        session.Caret = new TextPosition(0, 11);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindPrevious("xyz");

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindPrevious_WithEmptyTerm_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindPrevious("");

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FindPrevious_AfterFindNext_FindsPreviousOccurrence()
    {
        // arrange - regression test for FindPrevious not working after FindNext
        var (viewModel, session) = CreateViewModel(["test one test two test"]);
        viewModel.SetViewportHeight(5);

        // act - find first two occurrences, then go back
        viewModel.FindNext("test");
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);

        viewModel.FindNext("test");
        Assert.AreEqual(new TextPosition(0, 9), session.Anchor);

        viewModel.FindNext("test");
        Assert.AreEqual(new TextPosition(0, 18), session.Anchor);

        var result = viewModel.FindPrevious("test");

        // assert - should find the second occurrence, not the same one
        Assert.IsTrue(result);
        Assert.AreEqual(new TextPosition(0, 9), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 13), session.Caret);
    }

    [TestMethod]
    public void FindPreviousFromCurrent_WithNoLastSearch_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        var result = viewModel.FindPreviousFromCurrent();

        // assert
        Assert.IsFalse(result);
    }



    [TestMethod]
    public void FindPreviousFromCurrent_UsesLastSearchTerm()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        viewModel.SetViewportHeight(5);
        viewModel.FindNext("test");
        session.Caret = new TextPosition(0, 17);
        session.Anchor = session.Caret;

        // act
        var result = viewModel.FindPreviousFromCurrent();

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual(new TextPosition(0, 9), session.Anchor);
        Assert.AreEqual(new TextPosition(0, 13), session.Caret);
    }

    // Replace Tests
    [TestMethod]
    public void Replace_WithMatchingSelection_ReplacesAndFindsNext()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        viewModel.SetViewportHeight(5);
        viewModel.FindNext("test");

        // act
        var result = viewModel.Replace("test", "REPLACED");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual("REPLACED one test two", session.Document.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 13), session.Anchor);
    }

    [TestMethod]
    public void Replace_WithNoSelection_FindsNext()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.Replace("test", "REPLACED");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual("test one test two", session.Document.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 0), session.Anchor);
    }

    [TestMethod]
    public void Replace_WithNonMatchingSelection_FindsNext()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        session.Anchor = new TextPosition(0, 5);
        session.Caret = new TextPosition(0, 8);
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.Replace("test", "REPLACED");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual("test one test two", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Replace_CaseSensitive_MatchingCase_Replaces()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["TEST one TEST two"]);
        viewModel.SearchOptions = new SearchOptions { CaseSensitive = true };
        viewModel.SetViewportHeight(5);
        viewModel.FindNext("TEST");

        // act
        var result = viewModel.Replace("TEST", "replaced");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual("replaced one TEST two", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Replace_CaseSensitive_NonMatchingCase_DoesNotReplace()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two"]);
        viewModel.SearchOptions = new SearchOptions { CaseSensitive = true };
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 4);
        viewModel.SetViewportHeight(5);

        // act - selection is "test" but searching for "TEST" with case sensitive
        var result = viewModel.Replace("TEST", "replaced");

        // assert - no replacement because case doesn't match
        Assert.IsFalse(result);
        Assert.AreEqual("test one test two", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void Replace_WithNullSearchTerm_ThrowsArgumentNullException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => viewModel.Replace(null!, "replacement"));
    }

    [TestMethod]
    public void Replace_WithNullReplacement_ThrowsArgumentNullException()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => viewModel.Replace("search", null!));
    }

    // ReplaceAll Tests
    [TestMethod]
    public void ReplaceAll_ReplacesAllAndMarksDirty()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["test one test two test"]);
        viewModel.SetViewportHeight(5);
        Assert.IsFalse(viewModel.IsDirty);

        // act
        var count = viewModel.ReplaceAll("test", "X");

        // assert
        Assert.AreEqual(3, count);
        Assert.AreEqual("X one X two X", session.Document.Lines[0].Content);
        Assert.IsTrue(viewModel.IsDirty);
    }

    [TestMethod]
    public void ReplaceAll_WithNoMatches_ReturnsZero()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Hello World"]);
        viewModel.SetViewportHeight(5);

        // act
        var count = viewModel.ReplaceAll("xyz", "REPLACED");

        // assert
        Assert.AreEqual(0, count);
        Assert.AreEqual("Hello World", session.Document.Lines[0].Content);
    }

    [TestMethod]
    public void ReplaceAll_WithEmptySearchTerm_ReturnsZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        var count = viewModel.ReplaceAll("", "REPLACED");

        // assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void ReplaceAll_StoresLastSearchTerm()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["test one test"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.ReplaceAll("test", "X");

        // assert
        Assert.AreEqual("test", viewModel.LastSearchTerm);
    }

    // GoToLine Tests
    [TestMethod]
    public void GoToLine_ValidLine_MovesCaret()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.GoToLine(2);

        // assert
        Assert.AreEqual(new TextPosition(1, 0), session.Caret);
        Assert.AreEqual(new TextPosition(1, 0), session.Anchor);
    }

    [TestMethod]
    public void GoToLine_LineTooHigh_ClampsToLastLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.GoToLine(100);

        // assert
        Assert.AreEqual(new TextPosition(2, 0), session.Caret);
    }

    [TestMethod]
    public void GoToLine_LineTooLow_ClampsToFirstLine()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2", "Line 3"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.GoToLine(0);

        // assert
        Assert.AreEqual(new TextPosition(0, 0), session.Caret);
    }

    [TestMethod]
    public void GoToLine_ClearsSelection()
    {
        // arrange
        var (viewModel, session) = CreateViewModel(["Line 1", "Line 2"]);
        session.Anchor = new TextPosition(0, 0);
        session.Caret = new TextPosition(0, 6);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.GoToLine(2);

        // assert
        Assert.IsFalse(session.HasSelection);
    }

    // SearchOptions Tests
    [TestMethod]
    public void SearchOptions_DefaultValue_IsSearchOptionsDefault()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.AreEqual(SearchOptions.Default, viewModel.SearchOptions);
    }

    [TestMethod]
    public void SearchOptions_CanBeSet()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        var options = new SearchOptions { CaseSensitive = true };

        // act
        viewModel.SearchOptions = options;

        // assert
        Assert.IsTrue(viewModel.SearchOptions.CaseSensitive);
    }

    [TestMethod]
    public void SearchOptions_SetToNull_UsesDefault()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SearchOptions = new SearchOptions { CaseSensitive = true };

        // act
        viewModel.SearchOptions = null!;

        // assert
        Assert.AreEqual(SearchOptions.Default, viewModel.SearchOptions);
    }

    [TestMethod]
    public void FindNext_CaseSensitive_RespectsOption()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello HELLO hello"]);
        viewModel.SearchOptions = new SearchOptions { CaseSensitive = true };
        viewModel.SetViewportHeight(5);

        // act
        var result = viewModel.FindNext("HELLO");

        // assert
        Assert.IsTrue(result);
        Assert.AreEqual("HELLO", viewModel.LastSearchTerm);
    }

    // HasLastSearch Tests
    [TestMethod]
    public void HasLastSearch_InitiallyFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);

        // act & assert
        Assert.IsFalse(viewModel.HasLastSearch);
        Assert.IsNull(viewModel.LastSearchTerm);
    }

    [TestMethod]
    public void HasLastSearch_TrueAfterSearch()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello"]);
        viewModel.SetViewportHeight(5);

        // act
        viewModel.FindNext("Hello");

        // assert
        Assert.IsTrue(viewModel.HasLastSearch);
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
}
