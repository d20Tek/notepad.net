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
}
