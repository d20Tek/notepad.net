namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewportTests
{
    // Constructor tests
    [TestMethod]
    public void Constructor_WithDefaultParameters_SetsPropertiesToZero()
    {
        // act
        var viewport = new Viewport();

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
        Assert.AreEqual(0, viewport.VisibleLineCount);
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var firstVisibleLine = 10;
        var visibleLineCount = 25;

        // act
        var viewport = new Viewport(firstVisibleLine, visibleLineCount);

        // assert
        Assert.AreEqual(firstVisibleLine, viewport.FirstVisibleLine);
        Assert.AreEqual(visibleLineCount, viewport.VisibleLineCount);
    }

    // Reset tests
    [TestMethod]
    public void Reset_ResetsFirstVisibleLineToZero()
    {
        // arrange
        var viewport = new Viewport(10, 20);
        viewport.ScrollLines(5, 100);

        // act
        viewport.Reset();

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void Reset_ResetsHorizontalOffsetToZero()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(15);

        // act
        viewport.Reset();

        // assert
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void Reset_ResetsBothScrollPositions()
    {
        // arrange
        var viewport = new Viewport(0, 10);
        viewport.ScrollLines(20, 100);
        viewport.ScrollColumns(30);

        // act
        viewport.Reset();

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void Reset_DoesNotAffectVisibleLineCount()
    {
        // arrange
        var viewport = new Viewport(0, 25);
        viewport.ScrollLines(10, 100);

        // act
        viewport.Reset();

        // assert
        Assert.AreEqual(25, viewport.VisibleLineCount);
    }

    // SetVisibleLineCount tests
    [TestMethod]
    public void SetVisibleLineCount_WithValidCount_UpdatesProperty()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act
        viewport.SetVisibleLineCount(25);

        // assert
        Assert.AreEqual(25, viewport.VisibleLineCount);
    }

    [TestMethod]
    public void SetVisibleLineCount_WithZero_UpdatesProperty()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act
        viewport.SetVisibleLineCount(0);

        // assert
        Assert.AreEqual(0, viewport.VisibleLineCount);
    }

    // SetVerticalOffset tests
    [TestMethod]
    public void SetVerticalOffset_WithValidOffset_UpdatesFirstVisibleLine()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act
        viewport.SetVerticalOffset(15);

        // assert
        Assert.AreEqual(15, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void SetVerticalOffset_WithZero_SetsFirstVisibleLineToZero()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        viewport.SetVerticalOffset(0);

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
    }

    // SetHorizontalOffset tests
    [TestMethod]
    public void SetHorizontalOffset_WithValidOffset_UpdatesHorizontalOffset()
    {
        // arrange
        var viewport = new Viewport();

        // act
        viewport.SetHorizontalOffset(25);

        // assert
        Assert.AreEqual(25, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void SetHorizontalOffset_WithZero_SetsHorizontalOffsetToZero()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(20);

        // act
        viewport.SetHorizontalOffset(0);

        // assert
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    // ScrollLines tests
    [TestMethod]
    public void ScrollLines_WithPositiveDelta_ScrollsDown()
    {
        // arrange
        var viewport = new Viewport(5, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollLines(3, totalDocumentLines);

        // assert
        Assert.AreEqual(8, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollLines_WithNegativeDelta_ScrollsUp()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollLines(-3, totalDocumentLines);

        // assert
        Assert.AreEqual(7, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollLines_ClampsToZero_WhenScrollingPastStart()
    {
        // arrange
        var viewport = new Viewport(5, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollLines(-10, totalDocumentLines);

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollLines_ClampsToMax_WhenScrollingPastEnd()
    {
        // arrange
        var viewport = new Viewport(80, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollLines(20, totalDocumentLines);

        // assert
        Assert.AreEqual(90, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollLines_WithZeroDelta_DoesNotChangePosition()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollLines(0, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollLines_WithDocumentSmallerThanViewport_ClampsToZero()
    {
        // arrange
        var viewport = new Viewport(0, 25);
        var totalDocumentLines = 10;

        // act
        viewport.ScrollLines(5, totalDocumentLines);

        // assert
        Assert.AreEqual(0, viewport.FirstVisibleLine);
    }

    // ScrollPages tests
    [TestMethod]
    public void ScrollPages_WithPositiveDelta_ScrollsDownByPages()
    {
        // arrange
        var viewport = new Viewport(0, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollPages(2, totalDocumentLines);

        // assert
        Assert.AreEqual(20, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollPages_WithNegativeDelta_ScrollsUpByPages()
    {
        // arrange
        var viewport = new Viewport(30, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollPages(-2, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void ScrollPages_ClampsToMax_WhenScrollingPastEnd()
    {
        // arrange
        var viewport = new Viewport(80, 10);
        var totalDocumentLines = 100;

        // act
        viewport.ScrollPages(5, totalDocumentLines);

        // assert
        Assert.AreEqual(90, viewport.FirstVisibleLine);
    }

    // EnsureLineVisible tests
    [TestMethod]
    public void EnsureLineVisible_WhenLineAboveViewport_ScrollsUp()
    {
        // arrange
        var viewport = new Viewport(20, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(15, totalDocumentLines);

        // assert
        Assert.AreEqual(15, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_WhenLineBelowViewport_ScrollsDown()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(25, totalDocumentLines);

        // assert
        Assert.AreEqual(16, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_WhenLineAlreadyVisible_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(15, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_WithNegativeLineIndex_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(-1, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_WithLineIndexBeyondDocument_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(100, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_ClampsAfterAdjustment()
    {
        // arrange
        var viewport = new Viewport(0, 10);
        var totalDocumentLines = 15;

        // act
        viewport.EnsureLineVisible(14, totalDocumentLines);

        // assert
        Assert.AreEqual(5, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_AtFirstVisibleLine_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(10, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    [TestMethod]
    public void EnsureLineVisible_AtLastVisibleLine_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport(10, 10);
        var totalDocumentLines = 100;

        // act
        viewport.EnsureLineVisible(19, totalDocumentLines);

        // assert
        Assert.AreEqual(10, viewport.FirstVisibleLine);
    }

    // IsLineVisible tests
    [TestMethod]
    public void IsLineVisible_WhenLineInViewport_ReturnsTrue()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(15);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsLineVisible_WhenLineBeforeViewport_ReturnsFalse()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(5);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsLineVisible_WhenLineAfterViewport_ReturnsFalse()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(25);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsLineVisible_AtFirstVisibleLine_ReturnsTrue()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(10);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsLineVisible_AtLastVisibleLine_ReturnsTrue()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(19);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsLineVisible_JustAfterLastVisibleLine_ReturnsFalse()
    {
        // arrange
        var viewport = new Viewport(10, 10);

        // act
        var result = viewport.IsLineVisible(20);

        // assert
        Assert.IsFalse(result);
    }

    // HorizontalOffset tests
    [TestMethod]
    public void Constructor_InitializesHorizontalOffsetToZero()
    {
        // act
        var viewport = new Viewport();

        // assert
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    // ScrollColumns tests
    [TestMethod]
    public void ScrollColumns_WithPositiveDelta_IncreasesHorizontalOffset()
    {
        // arrange
        var viewport = new Viewport();

        // act
        viewport.ScrollColumns(10);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ScrollColumns_WithNegativeDelta_DecreasesHorizontalOffset()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(20);

        // act
        viewport.ScrollColumns(-5);

        // assert
        Assert.AreEqual(15, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ScrollColumns_ClampsToZero_WhenScrollingPastStart()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(5);

        // act
        viewport.ScrollColumns(-10);

        // assert
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void ScrollColumns_WithZeroDelta_DoesNotChangeOffset()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);

        // act
        viewport.ScrollColumns(0);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    // EnsureColumnVisible tests
    [TestMethod]
    public void EnsureColumnVisible_WhenColumnInViewport_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(50, viewportWidth);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_WhenColumnLeftOfViewport_ScrollsLeft()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(20);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(10, viewportWidth);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_WhenColumnRightOfViewport_ScrollsRight()
    {
        // arrange
        var viewport = new Viewport();
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(100, viewportWidth);

        // assert
        Assert.AreEqual(21, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_WithZeroViewportWidth_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);

        // act
        viewport.EnsureColumnVisible(50, 0);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_WithNegativeViewportWidth_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);

        // act
        viewport.EnsureColumnVisible(50, -10);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_AtLeftEdge_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(10, viewportWidth);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_AtRightEdge_DoesNotScroll()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(89, viewportWidth);

        // assert
        Assert.AreEqual(10, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_JustPastRightEdge_ScrollsRight()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(10);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(90, viewportWidth);

        // assert
        Assert.AreEqual(11, viewport.HorizontalOffset);
    }

    [TestMethod]
    public void EnsureColumnVisible_WithColumnZero_ScrollsToStart()
    {
        // arrange
        var viewport = new Viewport();
        viewport.ScrollColumns(50);
        var viewportWidth = 80;

        // act
        viewport.EnsureColumnVisible(0, viewportWidth);

        // assert
        Assert.AreEqual(0, viewport.HorizontalOffset);
    }
}
