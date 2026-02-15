namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
[ExcludeFromCodeCoverage]
public class LineEditingServiceExceptionTests
{
    [TestMethod]
    public void Insert_NullLine_ThrowsArgumentNullException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Insert(null!, 0, "text"));
    }

    [TestMethod]
    public void Insert_NullText_ThrowsArgumentNullException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Insert(line, 0, null!));
    }

    [TestMethod]
    public void Insert_NegativeColumn_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Insert(line, -1, "text"));
    }

    [TestMethod]
    public void Insert_ColumnBeyondLength_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Insert(line, 10, "text"));
    }

    [TestMethod]
    public void Delete_NullLine_ThrowsArgumentNullException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Delete(null!, 0));
    }

    [TestMethod]
    public void Delete_NegativeColumn_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Delete(line, -1));
    }

    [TestMethod]
    public void Delete_ColumnBeyondLength_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Delete(line, 10));
    }

    [TestMethod]
    public void Split_NullLine_ThrowsArgumentNullException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Split(null!, 0));
    }

    [TestMethod]
    public void Split_NegativeColumn_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Split(line, -1));
    }

    [TestMethod]
    public void Split_ColumnBeyondLength_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var line = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LineEditingService.Split(line, 10));
    }

    [TestMethod]
    public void Merge_NullLeft_ThrowsArgumentNullException()
    {
        // arrange
        var right = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Merge(null!, right));
    }

    [TestMethod]
    public void Merge_NullRight_ThrowsArgumentNullException()
    {
        // arrange
        var left = new TextLine("Hello");

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => LineEditingService.Merge(left, null!));
    }
}
