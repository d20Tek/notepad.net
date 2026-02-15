using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Document;

[TestClass]
[ExcludeFromCodeCoverage]
public class DocumentExceptionTests
{
    [TestMethod]
    public void Constructor_NullLines_ThrowsArgumentNullException()
    {
        // arrange

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new Doc.Document(null!));
    }

    [TestMethod]
    public void Constructor_NullEncoding_ThrowsArgumentNullException()
    {
        // arrange
        var lines = new[] { new TextLine("Test") };

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new Doc.Document(lines, null!, LineEndingStyle.CRLF));
    }

    [TestMethod]
    public void ReplaceLines_NegativeStartIndex_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => doc.ReplaceLines(-1, 1, []));
    }

    [TestMethod]
    public void ReplaceLines_StartIndexBeyondCount_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => doc.ReplaceLines(5, 0, []));
    }

    [TestMethod]
    public void ReplaceLines_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => doc.ReplaceLines(0, -1, []));
    }


    [TestMethod]
    public void ReplaceLines_CountExceedsBounds_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => doc.ReplaceLines(0, 5, []));
    }

    [TestMethod]
    public void ReplaceLines_NullNewLines_ThrowsArgumentNullException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => doc.ReplaceLines(0, 1, null!));
    }

    [TestMethod]
    public void SetEncoding_NullEncoding_ThrowsArgumentNullException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => doc.SetEncoding(null!));
    }
}
