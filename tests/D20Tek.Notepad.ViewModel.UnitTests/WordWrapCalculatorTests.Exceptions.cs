namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class WordWrapCalculatorExceptionsTests
{
    [TestMethod]
    public void WrapLine_WithNullText_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.WrapLine(null!, 80));
    }

    [TestMethod]
    public void WrapLine_WithZeroViewportWidth_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.WrapLine("text", 0));
    }

    [TestMethod]
    public void WrapLine_WithNegativeViewportWidth_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.WrapLine("text", -1));
    }

    [TestMethod]
    public void CountSegments_WithNullText_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.CountSegments(null!, 80));
    }

    [TestMethod]
    public void CountSegments_WithZeroViewportWidth_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.CountSegments("text", 0));
    }

    [TestMethod]
    public void CountSegments_WithNegativeViewportWidth_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => WordWrapCalculator.CountSegments("text", -1));
    }
}
