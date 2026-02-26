using D20Tek.Notepad.Core.Search;

namespace D20Tek.Notepad.Core.UnitTests.Search;

[TestClass]
public class SearchServiceTests
{
    private static readonly DocumentFactory _docFactory = new();

    // FindNext Tests - Basic
    [TestMethod]
    public void FindNext_WithMatchOnSameLine_ReturnsMatch()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "World", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 6), result.Start);
        Assert.AreEqual(new TextPosition(0, 11), result.End);
        Assert.AreEqual(5, result.Length);
    }

    [TestMethod]
    public void FindNext_WithMatchOnNextLine_ReturnsMatch()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello", "World"]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "World", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(1, 0), result.Start);
        Assert.AreEqual(new TextPosition(1, 5), result.End);
    }

    [TestMethod]
    public void FindNext_WithNoMatch_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "XYZ", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindNext_WithEmptySearchTerm_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindNext_FromMiddleOfLine_FindsMatchAfterPosition()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["test test test"]);
        var fromPosition = new TextPosition(0, 5);

        // act
        var result = service.FindNext(document, "test", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 5), result.Start);
    }

    [TestMethod]
    public void FindNext_FromAfterLastMatch_WithWrapAround_WrapsToBeginning()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["test one", "test two"]);
        var fromPosition = new TextPosition(1, 5);
        var options = new SearchOptions { WrapAround = true };

        // act
        var result = service.FindNext(document, "test", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindNext_FromAfterLastMatch_WithoutWrapAround_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["hello world"]);
        var fromPosition = new TextPosition(0, 6);
        var options = new SearchOptions { WrapAround = false };

        // act
        var result = service.FindNext(document, "hello", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    // FindNext Tests - Case Sensitivity
    [TestMethod]
    public void FindNext_CaseSensitive_RespectsCase()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello HELLO hello"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { CaseSensitive = true };

        // act
        var result = service.FindNext(document, "HELLO", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 6), result.Start);
    }

    [TestMethod]
    public void FindNext_CaseInsensitive_IgnoresCase()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello HELLO hello"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { CaseSensitive = false };

        // act
        var result = service.FindNext(document, "hello", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindNext_CaseSensitive_NoMatch_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { CaseSensitive = true };

        // act
        var result = service.FindNext(document, "hello", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    // FindPrevious Tests
    [TestMethod]
    public void FindPrevious_WithMatch_ReturnsMatch()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 11);

        // act
        var result = service.FindPrevious(document, "Hello", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
        Assert.AreEqual(new TextPosition(0, 5), result.End);
    }

    [TestMethod]
    public void FindPrevious_WithMatchOnPreviousLine_ReturnsMatch()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello", "World"]);
        var fromPosition = new TextPosition(1, 5);

        // act
        var result = service.FindPrevious(document, "Hello", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindPrevious_WithNoMatch_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 11);

        // act
        var result = service.FindPrevious(document, "XYZ", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindPrevious_FromBeforeFirstMatch_WithWrapAround_WrapsToEnd()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["one test", "two test"]);
        var fromPosition = new TextPosition(0, 3);
        var options = new SearchOptions { WrapAround = true };

        // act
        var result = service.FindPrevious(document, "test", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(1, 4), result.Start);
    }

    [TestMethod]
    public void FindPrevious_FromBeforeFirstMatch_WithoutWrapAround_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["one test"]);
        var fromPosition = new TextPosition(0, 3);
        var options = new SearchOptions { WrapAround = false };

        // act
        var result = service.FindPrevious(document, "test", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindPrevious_WithEmptySearchTerm_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello World"]);
        var fromPosition = new TextPosition(0, 11);

        // act
        var result = service.FindPrevious(document, "", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindPrevious_CaseSensitive_RespectsCase()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["hello HELLO Hello"]);
        var fromPosition = new TextPosition(0, 17);
        var options = new SearchOptions { CaseSensitive = true };

        // act
        var result = service.FindPrevious(document, "HELLO", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 6), result.Start);
    }

    // SearchResult Tests
    [TestMethod]
    public void SearchResult_NotFound_HasCorrectProperties()
    {
        // arrange & act
        var result = SearchResult.NotFound;

        // assert
        Assert.IsFalse(result.Found);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void SearchResult_Match_HasCorrectLength()
    {
        // arrange & act
        var result = SearchResult.Match(new TextPosition(0, 5), new TextPosition(0, 10));

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(5, result.Length);
    }

    // SearchOptions Tests
    [TestMethod]
    public void SearchOptions_Default_HasExpectedValues()
    {
        // arrange & act
        var options = SearchOptions.Default;

        // assert
        Assert.IsFalse(options.CaseSensitive);
        Assert.IsTrue(options.WrapAround);
    }

    // Edge Cases
    [TestMethod]
    public void FindNext_EmptyDocument_ReturnsNotFound()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument([""]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "test", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindNext_PositionBeyondDocumentEnd_ClampsPosition()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["Hello"]);
        var fromPosition = new TextPosition(100, 100);

        // act
        var result = service.FindNext(document, "Hello", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindNext_MultipleMatchesOnSameLine_FindsFirst()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["test test test"]);
        var fromPosition = new TextPosition(0, 0);

        // act
        var result = service.FindNext(document, "test", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindPrevious_MultipleMatchesOnSameLine_FindsLast()
    {
        // arrange
        var service = new SearchService();
        var document = CreateDocument(["test test test"]);
        var fromPosition = new TextPosition(0, 14);

        // act
        var result = service.FindPrevious(document, "test", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 10), result.Start);
    }

    // Branch Coverage: FindNext wrap-around when found but position >= fromPosition
    [TestMethod]
    public void FindNext_WrapAround_FoundButPositionNotLessThan_ReturnsNotFound()
    {
        // arrange - search from position 0, wrap finds match at position 0 (not < fromPosition)
        var service = new SearchService();
        var document = CreateDocument(["hello"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { WrapAround = true };

        // act - searching for "xyz" which doesn't exist, wrap should not find anything
        var result = service.FindNext(document, "xyz", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindNext_WrapAround_FoundAtEarlierPosition_ReturnsMatch()
    {
        // arrange - search from after only match, must wrap to find it
        var service = new SearchService();
        var document = CreateDocument(["hello world"]);
        var fromPosition = new TextPosition(0, 6);
        var options = new SearchOptions { WrapAround = true };

        // act - from position 6 ("world"), no "hello" after, wraps and finds at 0
        var result = service.FindNext(document, "hello", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    // Branch Coverage: SearchBackward searchEndColumn <= 0
    [TestMethod]
    public void FindPrevious_FromColumnZero_SkipsCurrentLine()
    {
        // arrange - search backward from column 0, should skip to previous line
        var service = new SearchService();
        var document = CreateDocument(["first", "second"]);
        var fromPosition = new TextPosition(1, 0);

        // act
        var result = service.FindPrevious(document, "first", fromPosition, SearchOptions.Default);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 0), result.Start);
    }

    [TestMethod]
    public void FindPrevious_FromColumnZeroOnFirstLine_ReturnsNotFoundWithoutWrap()
    {
        // arrange - at position (0,0), searchEndColumn = 0, line skipped
        var service = new SearchService();
        var document = CreateDocument(["hello"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { WrapAround = false };

        // act
        var result = service.FindPrevious(document, "hello", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    [TestMethod]
    public void FindPrevious_FromColumnZeroOnFirstLine_WithWrapAround_FindsAtEnd()
    {
        // arrange - at position (0,0), wraps to end and finds match
        var service = new SearchService();
        var document = CreateDocument(["hello world"]);
        var fromPosition = new TextPosition(0, 0);
        var options = new SearchOptions { WrapAround = true };

        // act
        var result = service.FindPrevious(document, "world", fromPosition, options);

        // assert
        Assert.IsTrue(result.Found);
        Assert.AreEqual(new TextPosition(0, 6), result.Start);
    }

    // Branch Coverage: FindPrevious wrap-around when found but position <= fromPosition
    [TestMethod]
    public void FindPrevious_WrapAround_FoundButPositionNotGreater_ReturnsNotFound()
    {
        // arrange - at end of doc, wrap finds match at start (position not > fromPosition fails)
        var service = new SearchService();
        var document = CreateDocument(["xyz"]);
        var fromPosition = new TextPosition(0, 3);
        var options = new SearchOptions { WrapAround = true };

        // act - searching for "abc" which doesn't exist
        var result = service.FindPrevious(document, "abc", fromPosition, options);

        // assert
        Assert.IsFalse(result.Found);
    }

    private static IDocument CreateDocument(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        return _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
    }
}
