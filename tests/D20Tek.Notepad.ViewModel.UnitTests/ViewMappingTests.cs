namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewMappingTests
{
    // DocumentToView tests
    [TestMethod]
    public void DocumentToView_WithValidPosition_ReturnsCorrectViewPosition()
    {
        // arrange
        var docPos = new TextPosition(10, 5);
        var firstVisibleLine = 5;

        // act
        var result = ViewMapping.DocumentToView(docPos, firstVisibleLine);

        // assert
        Assert.AreEqual(5, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void DocumentToView_WithFirstVisibleLine_ReturnsLineIndexZero()
    {
        // arrange
        var docPos = new TextPosition(5, 10);
        var firstVisibleLine = 5;

        // act
        var result = ViewMapping.DocumentToView(docPos, firstVisibleLine);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void DocumentToView_WithZeroFirstVisibleLine_ReturnsDocumentLine()
    {
        // arrange
        var docPos = new TextPosition(3, 7);
        var firstVisibleLine = 0;

        // act
        var result = ViewMapping.DocumentToView(docPos, firstVisibleLine);

        // assert
        Assert.AreEqual(3, result.LineIndex);
        Assert.AreEqual(7, result.Column);
    }

    [TestMethod]
    public void DocumentToView_WithLineBeforeFirstVisible_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var docPos = new TextPosition(3, 5);
        var firstVisibleLine = 5;

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => ViewMapping.DocumentToView(docPos, firstVisibleLine));
    }

    // ViewToDocument tests
    [TestMethod]
    public void ViewToDocument_WithValidPosition_ReturnsCorrectDocumentPosition()
    {
        // arrange
        var viewPos = new ViewPosition(5, 10);
        var firstVisibleLine = 10;

        // act
        var result = ViewMapping.ViewToDocument(viewPos, firstVisibleLine);

        // assert
        Assert.AreEqual(15, result.Line);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ViewToDocument_WithZeroLineIndex_ReturnsFirstVisibleLine()
    {
        // arrange
        var viewPos = new ViewPosition(0, 5);
        var firstVisibleLine = 10;

        // act
        var result = ViewMapping.ViewToDocument(viewPos, firstVisibleLine);

        // assert
        Assert.AreEqual(10, result.Line);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ViewToDocument_WithZeroFirstVisibleLine_ReturnsViewLineIndex()
    {
        // arrange
        var viewPos = new ViewPosition(3, 7);
        var firstVisibleLine = 0;

        // act
        var result = ViewMapping.ViewToDocument(viewPos, firstVisibleLine);

        // assert
        Assert.AreEqual(3, result.Line);
        Assert.AreEqual(7, result.Column);
    }

    // DocumentSelectionToView tests
    [TestMethod]
    public void DocumentSelectionToView_WithForwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var anchor = new TextPosition(10, 5);
        var caret = new TextPosition(12, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = ViewMapping.DocumentSelectionToView(anchor, caret, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.Start);
        Assert.AreEqual(new ViewPosition(2, 10), result.Value.End);
    }

    [TestMethod]
    public void DocumentSelectionToView_WithBackwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var anchor = new TextPosition(12, 10);
        var caret = new TextPosition(10, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = ViewMapping.DocumentSelectionToView(anchor, caret, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.Start);
        Assert.AreEqual(new ViewPosition(2, 10), result.Value.End);
    }

    [TestMethod]
    public void DocumentSelectionToView_WithSamePosition_ReturnsEmptyRange()
    {
        // arrange
        var position = new TextPosition(10, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = ViewMapping.DocumentSelectionToView(position, position, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Value.IsEmpty);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.Start);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.End);
    }

    [TestMethod]
    public void DocumentSelectionToView_WithSameLineBackwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var anchor = new TextPosition(10, 15);
        var caret = new TextPosition(10, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = ViewMapping.DocumentSelectionToView(anchor, caret, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.Start);
        Assert.AreEqual(new ViewPosition(0, 15), result.Value.End);
    }

    // ClampToViewport tests
    [TestMethod]
    public void ClampToViewport_WithPositionInViewport_ReturnsCorrectViewPosition()
    {
        // arrange
        var docPos = new TextPosition(15, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(5, result.LineIndex);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithPositionBeforeViewport_ClampsToFirstLine()
    {
        // arrange
        var docPos = new TextPosition(5, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithPositionAfterViewport_ClampsToLastLine()
    {
        // arrange
        var docPos = new TextPosition(50, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(19, result.LineIndex);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithPositionAtFirstVisibleLine_ReturnsLineIndexZero()
    {
        // arrange
        var docPos = new TextPosition(10, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithPositionAtLastVisibleLine_ReturnsLastLineIndex()
    {
        // arrange
        var docPos = new TextPosition(29, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(19, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithSingleLineViewport_ClampsToZero()
    {
        // arrange
        var docPos = new TextPosition(15, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 1;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_WithZeroVisibleLineCount_ReturnsZeroLineIndex()
    {
        // arrange
        var docPos = new TextPosition(15, 8);
        var firstVisibleLine = 10;
        var visibleLineCount = 0;

        // act
        var result = ViewMapping.ClampToViewport(docPos, firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(0, result.LineIndex);
        Assert.AreEqual(8, result.Column);
    }

    // ============================================================
    // Extension Method Tests
    // ============================================================

    [TestMethod]
    public void ToViewPosition_WithValidPosition_ReturnsCorrectViewPosition()
    {
        // arrange
        var docPos = new TextPosition(10, 5);
        var firstVisibleLine = 5;

        // act
        var result = docPos.ToViewPosition(firstVisibleLine);

        // assert
        Assert.AreEqual(5, result.LineIndex);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void ToDocumentPosition_WithValidPosition_ReturnsCorrectDocumentPosition()
    {
        // arrange
        var viewPos = new ViewPosition(5, 10);
        var firstVisibleLine = 10;

        // act
        var result = viewPos.ToDocumentPosition(firstVisibleLine);

        // assert
        Assert.AreEqual(15, result.Line);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ClampToViewport_ExtensionMethod_WithPositionInViewport_ReturnsCorrectViewPosition()
    {
        // arrange
        var docPos = new TextPosition(15, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 20;

        // act
        var result = docPos.ClampToViewport(firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(5, result.LineIndex);
        Assert.AreEqual(10, result.Column);
    }

    [TestMethod]
    public void ToViewSelectionRange_WithForwardSelection_ReturnsNormalizedRange()
    {
        // arrange
        var anchor = new TextPosition(10, 5);
        var caret = new TextPosition(12, 10);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = anchor.ToViewSelectionRange(caret, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNotNull(result);
        Assert.AreEqual(new ViewPosition(0, 5), result.Value.Start);
        Assert.AreEqual(new ViewPosition(2, 10), result.Value.End);
    }

    [TestMethod]
    public void ToViewSelectionRange_WithSelectionOutsideViewport_ReturnsNull()
    {
        // arrange
        var anchor = new TextPosition(0, 0);
        var caret = new TextPosition(5, 5);
        var firstVisibleLine = 10;
        var visibleLineCount = 10;

        // act
        var result = anchor.ToViewSelectionRange(caret, firstVisibleLine, visibleLineCount);

        // assert
        Assert.IsNull(result);
    }
}
