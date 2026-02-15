namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class LineEditingServiceTests
{
    // Insert tests
    [TestMethod]
    public void Insert_AtMiddle_InsertsTextAndReturnsNewColumn()
    {
        // arrange
        var line = new TextLine("HelloWorld");

        // act
        var (newLine, newColumn) = LineEditingService.Insert(line, 5, " ");

        // assert
        Assert.AreEqual("Hello World", newLine.Content);
        Assert.AreEqual(6, newColumn);
    }

    [TestMethod]
    public void Insert_AtStart_InsertsTextAtBeginning()
    {
        // arrange
        var line = new TextLine("World");

        // act
        var (newLine, newColumn) = LineEditingService.Insert(line, 0, "Hello ");

        // assert
        Assert.AreEqual("Hello World", newLine.Content);
        Assert.AreEqual(6, newColumn);
    }

    [TestMethod]
    public void Insert_AtEnd_AppendsText()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (newLine, newColumn) = LineEditingService.Insert(line, 5, " World");

        // assert
        Assert.AreEqual("Hello World", newLine.Content);
        Assert.AreEqual(11, newColumn);
    }

    [TestMethod]
    public void Insert_EmptyText_ReturnsUnchangedLine()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (newLine, newColumn) = LineEditingService.Insert(line, 3, "");

        // assert
        Assert.AreEqual("Hello", newLine.Content);
        Assert.AreEqual(3, newColumn);
    }

    // Delete tests
    [TestMethod]
    public void Delete_AtMiddle_RemovesCharacterBeforeColumn()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (newLine, newColumn) = LineEditingService.Delete(line, 3);

        // assert
        Assert.AreEqual("Helo", newLine.Content);
        Assert.AreEqual(2, newColumn);
    }

    [TestMethod]
    public void Delete_AtEnd_RemovesLastCharacter()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (newLine, newColumn) = LineEditingService.Delete(line, 5);

        // assert
        Assert.AreEqual("Hell", newLine.Content);
        Assert.AreEqual(4, newColumn);
    }

    [TestMethod]
    public void Delete_AtStart_ReturnsUnchangedLine()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (newLine, newColumn) = LineEditingService.Delete(line, 0);

        // assert
        Assert.AreEqual("Hello", newLine.Content);
        Assert.AreEqual(0, newColumn);
    }

    // Split tests
    [TestMethod]
    public void Split_AtMiddle_SplitsLineIntoTwoParts()
    {
        // arrange
        var line = new TextLine("HelloWorld");

        // act
        var (left, right) = LineEditingService.Split(line, 5);

        // assert
        Assert.AreEqual("Hello", left.Content);
        Assert.AreEqual("World", right.Content);
    }

    [TestMethod]
    public void Split_AtStart_ReturnsEmptyLeftAndFullRight()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (left, right) = LineEditingService.Split(line, 0);

        // assert
        Assert.AreEqual(string.Empty, left.Content);
        Assert.AreEqual("Hello", right.Content);
    }

    [TestMethod]
    public void Split_AtEnd_ReturnsFullLeftAndEmptyRight()
    {
        // arrange
        var line = new TextLine("Hello");

        // act
        var (left, right) = LineEditingService.Split(line, 5);

        // assert
        Assert.AreEqual("Hello", left.Content);
        Assert.AreEqual(string.Empty, right.Content);
    }

    // Merge tests
    [TestMethod]
    public void Merge_TwoLines_CombinesContent()
    {
        // arrange
        var left = new TextLine("Hello ");
        var right = new TextLine("World");

        // act
        var result = LineEditingService.Merge(left, right);

        // assert
        Assert.AreEqual("Hello World", result.Content);
    }

    [TestMethod]
    public void Merge_EmptyLeft_ReturnsRightContent()
    {
        // arrange
        var left = new TextLine(string.Empty);
        var right = new TextLine("Hello");

        // act
        var result = LineEditingService.Merge(left, right);

        // assert
        Assert.AreEqual("Hello", result.Content);
    }

    [TestMethod]
    public void Merge_EmptyRight_ReturnsLeftContent()
    {
        // arrange
        var left = new TextLine("Hello");
        var right = new TextLine(string.Empty);

        // act
        var result = LineEditingService.Merge(left, right);

        // assert
        Assert.AreEqual("Hello", result.Content);
    }

    [TestMethod]
    public void Merge_BothEmpty_ReturnsEmptyLine()
    {
        // arrange
        var left = new TextLine(string.Empty);
        var right = new TextLine(string.Empty);

        // act
        var result = LineEditingService.Merge(left, right);

        // assert
        Assert.AreEqual(string.Empty, result.Content);
    }
}
