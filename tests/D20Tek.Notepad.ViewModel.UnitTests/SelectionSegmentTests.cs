namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class SelectionSegmentTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var startColumn = 5;
        var endColumn = 15;

        // act
        var segment = new SelectionSegment(startColumn, endColumn);

        // assert
        Assert.AreEqual(startColumn, segment.StartColumn);
        Assert.AreEqual(endColumn, segment.EndColumn);
    }

    [TestMethod]
    public void Constructor_WithZeroValues_SetsProperties()
    {
        // arrange & act
        var segment = new SelectionSegment(0, 0);

        // assert
        Assert.AreEqual(0, segment.StartColumn);
        Assert.AreEqual(0, segment.EndColumn);
    }

    [TestMethod]
    public void Constructor_WithSameStartAndEnd_SetsProperties()
    {
        // arrange & act
        var segment = new SelectionSegment(10, 10);

        // assert
        Assert.AreEqual(10, segment.StartColumn);
        Assert.AreEqual(10, segment.EndColumn);
    }

    [TestMethod]
    public void Constructor_WithLargeValues_SetsProperties()
    {
        // arrange
        var startColumn = 1000;
        var endColumn = 5000;

        // act
        var segment = new SelectionSegment(startColumn, endColumn);

        // assert
        Assert.AreEqual(startColumn, segment.StartColumn);
        Assert.AreEqual(endColumn, segment.EndColumn);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        // arrange
        var segment = new SelectionSegment(5, 15);

        // act
        var result = segment.ToString();

        // assert
        Assert.AreEqual("[5..15]", result);
    }

    [TestMethod]
    public void ToString_WithZeroValues_ReturnsFormattedString()
    {
        // arrange
        var segment = new SelectionSegment(0, 0);

        // act
        var result = segment.ToString();

        // assert
        Assert.AreEqual("[0..0]", result);
    }

    [TestMethod]
    public void ToString_WithSameStartAndEnd_ReturnsFormattedString()
    {
        // arrange
        var segment = new SelectionSegment(10, 10);

        // act
        var result = segment.ToString();

        // assert
        Assert.AreEqual("[10..10]", result);
    }
}