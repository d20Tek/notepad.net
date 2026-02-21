namespace D20Tek.Notepad.Core.UnitTests.Primitives;

[TestClass]
public class TextLineTests
{
    [TestMethod]
    public void Constructor_SetsContent()
    {
        // arrange
        var expected = "line content";

        // act
        var line = new TextLine(expected);

        // assert
        Assert.AreEqual(expected, line.Content);
    }

    [TestMethod]
    public void Length_ReturnsContentLength()
    {
        // arrange
        var content = "Hello World";
        var line = new TextLine(content);

        // act
        var length = line.Length;

        // assert
        Assert.AreEqual(11, length);
    }

    [TestMethod]
    public void Length_WithEmptyContent_ReturnsZero()
    {
        // arrange
        var line = new TextLine(string.Empty);

        // act
        var length = line.Length;

        // assert
        Assert.AreEqual(0, length);
    }

    [TestMethod]
    public void Empty_ReturnsTextLineWithEmptyContent()
    {
        // act
        var line = TextLine.Empty;

        // assert
        Assert.AreEqual(string.Empty, line.Content);
        Assert.AreEqual(0, line.Length);
    }
}
