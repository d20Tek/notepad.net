namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewLineTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var lineIndex = 5;
        var text = "Hello World";

        // act
        var viewLine = new ViewLine(lineIndex, text);

        // assert
        Assert.AreEqual(lineIndex, viewLine.DocumentLineIndex);
        Assert.AreEqual(text, viewLine.Text);
    }

    [TestMethod]
    public void Constructor_WithZeroIndex_SetsProperties()
    {
        // arrange
        var lineIndex = 0;
        var text = "First line";

        // act
        var viewLine = new ViewLine(lineIndex, text);

        // assert
        Assert.AreEqual(0, viewLine.DocumentLineIndex);
        Assert.AreEqual(text, viewLine.Text);
    }

    [TestMethod]
    public void Constructor_WithEmptyText_SetsProperties()
    {
        // arrange
        var lineIndex = 0;
        var text = string.Empty;

        // act
        var viewLine = new ViewLine(lineIndex, text);

        // assert
        Assert.AreEqual(lineIndex, viewLine.DocumentLineIndex);
        Assert.AreEqual(string.Empty, viewLine.Text);
    }

    [TestMethod]
    public void Constructor_WithNegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>([ExcludeFromCodeCoverage]() => new ViewLine(-1, "text"));
    }

    [TestMethod]
    public void Constructor_WithNullText_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() => new ViewLine(0, null!));
    }

    [TestMethod]
    public void ToString_ReturnsText()
    {
        // arrange
        var text = "Hello World";
        var viewLine = new ViewLine(0, text);

        // act
        var result = viewLine.ToString();

        // assert
        Assert.AreEqual(text, result);
    }

    [TestMethod]
    public void ToString_WithEmptyText_ReturnsEmptyString()
    {
        // arrange
        var viewLine = new ViewLine(0, string.Empty);

        // act
        var result = viewLine.ToString();

        // assert
        Assert.AreEqual(string.Empty, result);
    }
}
