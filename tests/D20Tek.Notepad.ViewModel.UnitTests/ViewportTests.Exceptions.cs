namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewportExceptionsTests
{
    [TestMethod]
    public void Constructor_WithNegativeFirstVisibleLine_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => new Viewport(-1, 10));
    }

    [TestMethod]
    public void Constructor_WithNegativeVisibleLineCount_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => new Viewport(0, -1));
    }

    [TestMethod]
    public void SetVisibleLineCount_WithNegativeCount_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => viewport.SetVisibleLineCount(-1));
    }

    [TestMethod]
    public void ScrollLines_WithNegativeTotalDocumentLines_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => viewport.ScrollLines(5, -1));
    }

    [TestMethod]
    public void SetVerticalOffset_WithNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var viewport = new Viewport(0, 10);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => viewport.SetVerticalOffset(-1));
    }

    [TestMethod]
    public void SetHorizontalOffset_WithNegativeOffset_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var viewport = new Viewport();

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => viewport.SetHorizontalOffset(-1));
    }
}
