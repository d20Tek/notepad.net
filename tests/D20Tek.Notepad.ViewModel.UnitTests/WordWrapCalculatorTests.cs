namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class WordWrapCalculatorTests
{
    [TestMethod]
    public void WrapLine_WithEmptyString_ReturnsSingleEmptySegment()
    {
        // arrange
        var text = string.Empty;
        var viewportWidth = 80;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(1, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(0, result[0].Length);
        Assert.AreEqual(string.Empty, result[0].Text);
    }

    [TestMethod]
    public void WrapLine_WithLineShorterThanViewport_ReturnsSingleSegment()
    {
        // arrange
        var text = "Hello World";
        var viewportWidth = 80;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(1, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(text.Length, result[0].Length);
        Assert.AreEqual(text, result[0].Text);
    }

    [TestMethod]
    public void WrapLine_WithLineExactlyViewportWidth_ReturnsSingleSegment()
    {
        // arrange
        var text = "1234567890";
        var viewportWidth = 10;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(1, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(10, result[0].Length);
        Assert.AreEqual(text, result[0].Text);
    }

    [TestMethod]
    public void WrapLine_WithLineLongerThanViewport_WrapsAtWordBoundary()
    {
        // arrange
        var text = "Hello World Today";
        var viewportWidth = 12;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(2, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual("Hello World", result[0].Text);
        Assert.AreEqual(12, result[1].StartColumn);
        Assert.AreEqual("Today", result[1].Text);
    }

    [TestMethod]
    public void WrapLine_WithLongWordExceedingViewport_WrapsAtCharacterBoundary()
    {
        // arrange
        var text = "Supercalifragilisticexpialidocious";
        var viewportWidth = 10;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(4, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual("Supercalif", result[0].Text);
        Assert.AreEqual(10, result[1].StartColumn);
        Assert.AreEqual("ragilistic", result[1].Text);
        Assert.AreEqual(20, result[2].StartColumn);
        Assert.AreEqual("expialidoc", result[2].Text);
        Assert.AreEqual(30, result[3].StartColumn);
        Assert.AreEqual("ious", result[3].Text);
    }

    [TestMethod]
    public void WrapLine_WithMultipleSpaces_SkipsLeadingSpacesOnNewLine()
    {
        // arrange
        var text = "Hello    World";
        var viewportWidth = 10;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(2, result);
        Assert.AreEqual("Hello", result[0].Text);
        Assert.AreEqual("World", result[1].Text);
        Assert.AreEqual(9, result[1].StartColumn);
    }

    [TestMethod]
    public void WrapLine_WithMultipleWraps_ReturnsCorrectSegments()
    {
        // arrange
        var text = "The quick brown fox jumps over the lazy dog";
        var viewportWidth = 16;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(3, result);
        Assert.AreEqual("The quick brown", result[0].Text);
        Assert.AreEqual("fox jumps over", result[1].Text);
        Assert.AreEqual("the lazy dog", result[2].Text);
    }

    [TestMethod]
    public void WrapLine_WithViewportWidthOne_WrapsEachCharacter()
    {
        // arrange
        var text = "ABC";
        var viewportWidth = 1;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(3, result);
        Assert.AreEqual("A", result[0].Text);
        Assert.AreEqual("B", result[1].Text);
        Assert.AreEqual("C", result[2].Text);
    }

    [TestMethod]
    public void WrapLine_WithTrailingSpaces_PreservesTrailingSpaces()
    {
        // arrange
        var text = "Hello   ";
        var viewportWidth = 80;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(1, result);
        Assert.AreEqual(text, result[0].Text);
    }

    [TestMethod]
    public void WrapLine_SegmentsHaveCorrectStartColumns()
    {
        // arrange
        var text = "One Two Three Four";
        var viewportWidth = 8;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(3, result);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(8, result[1].StartColumn);
        Assert.AreEqual(14, result[2].StartColumn);
    }

    [TestMethod]
    public void WrapLine_SegmentsHaveCorrectLengths()
    {
        // arrange
        var text = "One Two Three";
        var viewportWidth = 8;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(2, result);
        Assert.AreEqual(7, result[0].Length);
        Assert.AreEqual(5, result[1].Length);
    }

    [TestMethod]
    public void WrapLine_WithOnlyTrailingSpacesAfterBreak_ReturnsSingleSegment()
    {
        // arrange
        var text = "Hello     ";
        var viewportWidth = 6;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);



        // assert
        Assert.HasCount(1, result);
        Assert.AreEqual("Hello", result[0].Text);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(5, result[0].Length);
    }

    [TestMethod]
    public void WrapLine_WithLeadingSpacesExceedingViewport_SkipsToFirstWord()
    {
        // arrange
        // Tests the boundary condition where lastSpaceIndex decrements down to equal startIndex
        // "   ABCD" - LastIndexOf finds space at position 2, loop walks back to position 0
        // When lastSpaceIndex == startIndex (both 0), the while loop exits
        var text = "   ABCD";
        var viewportWidth = 5;

        // act
        var result = WordWrapCalculator.WrapLine(text, viewportWidth);

        // assert
        Assert.HasCount(2, result);
        Assert.AreEqual(string.Empty, result[0].Text);
        Assert.AreEqual(0, result[0].StartColumn);
        Assert.AreEqual(0, result[0].Length);
        Assert.AreEqual("ABCD", result[1].Text);
        Assert.AreEqual(3, result[1].StartColumn);
        Assert.AreEqual(4, result[1].Length);
    }
}
