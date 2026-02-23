using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewMappingHelperTests
{
    // ToWrappedViewPosition Tests
    [TestMethod]
    public void ToWrappedViewPosition_WithPositionInFirstSegment_ReturnsCorrectView()
    {
        // arrange
        // "Hello World" wrapped to "Hello", "World"
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 3); // "llo" in "Hello"

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(3, result.Column);
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithPositionInSecondSegment_ReturnsCorrectView()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 8); // "rld" in "World"

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(1, result.LineIndex);
        Assert.AreEqual(2, result.Column); // 8 - 6 = 2
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithPositionOnDifferentDocLine_ReturnsCorrectView()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (1, 0, "World")
        ]);
        var docPos = new TextPosition(1, 2);

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(1, result.LineIndex);
        Assert.AreEqual(2, result.Column);
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithPositionAtSegmentBoundary_ReturnsEndOfSegment()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 5); // End of "Hello"

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithPositionNotVisible_ReturnsLastVisiblePosition()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var docPos = new TextPosition(5, 10); // Beyond visible lines (different doc line)

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(5, result.Column); // End of last visible line
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithColumnInLaterSegmentNotVisible_ReturnsLastVisibleSegment()
    {
        // arrange
        // Document line 0 wraps to multiple segments, but only first two are visible
        // "Hello World Today" -> "Hello"(0-5), "World"(6-11), "Today"(12-17)
        // Only "Hello" and "World" segments are in visible lines
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 15); // Column 15 is in "Today" segment (not visible)

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        // Should return position at end of last visible segment for this document line
        Assert.AreEqual(1, result.LineIndex);
        Assert.AreEqual(5, result.Column); // "World".Length
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithEmptyVisibleLines_ReturnsZeroPosition()
    {
        // arrange
        var visibleLines = new List<ViewLine>();
        var docPos = new TextPosition(0, 5);

        // act
        var result = ViewMappingHelper.ToWrappedViewPosition(docPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(0, result.Column);
    }

    [TestMethod]
    public void ToWrappedViewPosition_WithNullVisibleLines_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => ViewMappingHelper.ToWrappedViewPosition(
                new TextPosition(0, 0), null!, 0));
    }

    // FromWrappedViewPosition Tests
    [TestMethod]
    public void FromWrappedViewPosition_WithFirstSegment_ReturnsCorrectDocPosition()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var viewPos = new ViewPosition(0, 3);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.Line);
        Assert.AreEqual(3, result.Column);
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithSecondSegment_ReturnsCorrectDocPosition()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var viewPos = new ViewPosition(1, 2);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.Line);
        Assert.AreEqual(8, result.Column); // 6 + 2
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithDifferentDocLine_ReturnsCorrectDocPosition()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (1, 0, "World")
        ]);
        var viewPos = new ViewPosition(1, 3);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(1, result.Line);
        Assert.AreEqual(3, result.Column);
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithColumnBeyondText_ClampsToTextLength()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var viewPos = new ViewPosition(0, 100);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.Line);
        Assert.AreEqual(5, result.Column); // Clamped to "Hello".Length
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithInvalidLineIndex_ReturnsFirstLinePosition()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var viewPos = new ViewPosition(5, 2);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.Line);
        Assert.AreEqual(0, result.Column);
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithEmptyVisibleLines_ReturnsZeroPosition()
    {
        // arrange
        var visibleLines = new List<ViewLine>();
        var viewPos = new ViewPosition(0, 0);

        // act
        var result = ViewMappingHelper.FromWrappedViewPosition(viewPos, visibleLines, 0);

        // assert
        Assert.AreEqual(0, result.Line);
        Assert.AreEqual(0, result.Column);
    }

    [TestMethod]
    public void FromWrappedViewPosition_WithNullVisibleLines_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => ViewMappingHelper.FromWrappedViewPosition(
                new ViewPosition(0, 0), null!, 0));
    }

    // GetViewLineIndexForDocumentPosition Tests
    [TestMethod]
    public void GetViewLineIndexForDocumentPosition_WithPositionInFirstSegment_ReturnsZero()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 3);

        // act
        var result = ViewMappingHelper.GetViewLineIndexForDocumentPosition(docPos, visibleLines);

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetViewLineIndexForDocumentPosition_WithPositionInSecondSegment_ReturnsOne()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello"),
            (0, 6, "World")
        ]);
        var docPos = new TextPosition(0, 8);

        // act
        var result = ViewMappingHelper.GetViewLineIndexForDocumentPosition(docPos, visibleLines);

        // assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void GetViewLineIndexForDocumentPosition_WithPositionNotVisible_ReturnsMinusOne()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var docPos = new TextPosition(5, 0);

        // act
        var result = ViewMappingHelper.GetViewLineIndexForDocumentPosition(docPos, visibleLines);

        // assert
        Assert.AreEqual(-1, result);
    }

    // IsDocumentPositionVisible Tests
    [TestMethod]
    public void IsDocumentPositionVisible_WithVisiblePosition_ReturnsTrue()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var docPos = new TextPosition(0, 3);

        // act
        var result = ViewMappingHelper.IsDocumentPositionVisible(docPos, visibleLines);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsDocumentPositionVisible_WithNotVisiblePosition_ReturnsFalse()
    {
        // arrange
        var visibleLines = CreateVisibleLines([
            (0, 0, "Hello")
        ]);
        var docPos = new TextPosition(5, 0);

        // act
        var result = ViewMappingHelper.IsDocumentPositionVisible(docPos, visibleLines);

        // assert
        Assert.IsFalse(result);
    }

    private static List<ViewLine> CreateVisibleLines((int docLine, int startCol, string text)[] lines)
    {
        return lines.Select(l => new ViewLine(l.docLine, l.startCol, l.text)).ToList();
    }
}
