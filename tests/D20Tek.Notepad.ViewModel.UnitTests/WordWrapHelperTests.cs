namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class WordWrapHelperTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void GetTotalVisualLineCount_WithShortLines_ReturnsDocumentLineCount()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var viewportWidth = 80;

        // act
        var result = WordWrapHelper.GetTotalVisualLineCount(doc, viewportWidth);

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetTotalVisualLineCount_WithWrappedLines_ReturnsTotalVisualLines()
    {
        // arrange
        // "Hello World" with width 6 -> "Hello", "World" (2 visual lines)
        var doc = CreateDocument(["Hello World", "Foo"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetTotalVisualLineCount(doc, viewportWidth);

        // assert
        Assert.AreEqual(3, result); // 2 from first line + 1 from second
    }

    [TestMethod]
    public void GetTotalVisualLineCount_WithZeroViewportWidth_ReturnsDocumentLineCount()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var viewportWidth = 0;

        // act
        var result = WordWrapHelper.GetTotalVisualLineCount(doc, viewportWidth);

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetTotalVisualLineCount_WithEmptyDocument_ReturnsOne()
    {
        // arrange
        var doc = CreateDocument([""]);
        var viewportWidth = 80;

        // act
        var result = WordWrapHelper.GetTotalVisualLineCount(doc, viewportWidth);

        // assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetTotalVisualLineCount_WithNullDocument_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => WordWrapHelper.GetTotalVisualLineCount(null!, 80));
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithNoWrapping_ReturnsDocumentLine()
    {
        // arrange
        var doc = CreateDocument(["Line 1", "Line 2", "Line 3"]);
        var viewportWidth = 80;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForDocumentLine(doc, viewportWidth, 2);

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithWrapping_ReturnsCorrectVisualIndex()
    {
        // arrange
        // Line 0: "Hello World" -> 2 visual lines
        // Line 1: "Foo" -> 1 visual line
        // Visual index for doc line 1 should be 2
        var doc = CreateDocument(["Hello World", "Foo"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForDocumentLine(doc, viewportWidth, 1);

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithZeroViewportWidth_ReturnsDocumentLine()
    {
        // arrange
        var doc = CreateDocument(["Hello World", "Foo"]);
        var viewportWidth = 0;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForDocumentLine(doc, viewportWidth, 1);

        // assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithNullDocument_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => WordWrapHelper.GetVisualLineIndexForDocumentLine(null!, 80, 0));
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithNegativeDocumentLine_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var doc = CreateDocument(["Hello"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => WordWrapHelper.GetVisualLineIndexForDocumentLine(doc, 80, -1));
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithColumnInFirstSegment_ReturnsBaseIndex()
    {
        // arrange
        // "Hello World" with width 6 -> "Hello", "World"
        // Column 3 is in first segment
        var doc = CreateDocument(["Hello World"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 0, 3);

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithColumnInSecondSegment_ReturnsCorrectIndex()
    {
        // arrange
        // "Hello World" with width 6 -> "Hello", "World"
        // Column 8 is in second segment
        var doc = CreateDocument(["Hello World"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 0, 8);

        // assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithSecondDocLine_IncludesPreviousLines()
    {
        // arrange
        // Line 0: "Hello World" -> 2 visual lines
        // Line 1: "Foo Bar" -> 2 visual lines
        // Position (1, 5) should be visual line 3 (0, 1 from first doc line, then 2, 3 from second)
        var doc = CreateDocument(["Hello World", "Foo Bar"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 1, 5);

        // assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithZeroViewportWidth_ReturnsDocumentLine()
    {
        // arrange
        var doc = CreateDocument(["Hello World"]);
        var viewportWidth = 0;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 0, 8);

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithNullDocument_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => WordWrapHelper.GetVisualLineIndexForPosition(null!, 80, 0, 0));
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithInvalidDocumentLine_ReturnsDocumentLine()
    {
        // arrange
        var doc = CreateDocument(["Hello"]);
        var viewportWidth = 80;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 5, 0);

        // assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithColumnInLastSegment_ReturnsLastSegmentIndex()
    {
        // arrange
        // "Hello World Today" with width 6 -> "Hello", "World", "Today" (3 segments)
        // Column 15 is in the last segment "Today"
        var doc = CreateDocument(["Hello World Today"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 0, 15);

        // assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void GetVisualLineIndexForPosition_WithColumnBeyondLineEnd_ReturnsLastSegmentIndex()
    {
        // arrange
        // "Hello World" with width 6 -> "Hello", "World" (2 segments)
        // Column 100 is beyond the line end, should return last segment
        var doc = CreateDocument(["Hello World"]);
        var viewportWidth = 6;

        // act
        var result = WordWrapHelper.GetVisualLineIndexForPosition(doc, viewportWidth, 0, 100);

        // assert
        Assert.AreEqual(1, result);
    }

    private static IDocument CreateDocument(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        return _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
    }
}
