using D20Tek.Notepad.Core.Document;
using D20Tek.Notepad.Core.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace D20Tek.Notepad.Core.UnitTests.Document;

[TestClass]
public class DocumentFactoryTests
{
    private readonly DocumentFactory _factory = new();

    [TestMethod]
    public void Create_WithValidData_ReturnsDocumentWithCorrectProperties()
    {
        // arrange
        var lines = new List<TextLine> { new("Hello"), new("World") };
        var data = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);

        // act
        var result = _factory.Create(data);

        // assert
        Assert.HasCount(2, result.Lines);
        Assert.AreEqual("Hello", result.Lines[0].Content);
        Assert.AreEqual("World", result.Lines[1].Content);
        Assert.AreEqual(Encoding.UTF8, result.Encoding);
        Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
    }

    [TestMethod]
    public void Create_WithEmptyLines_ReturnsDocumentWithSingleEmptyLine()
    {
        // arrange
        var data = new DocumentData([], Encoding.UTF8, LineEndingStyle.LF);

        // act
        var result = _factory.Create(data);

        // assert
        Assert.HasCount(1, result.Lines);
        Assert.AreEqual(string.Empty, result.Lines[0].Content);
    }

    [TestMethod]
    public void Create_WithDifferentEncoding_SetsEncodingCorrectly()
    {
        // arrange
        var data = new DocumentData([new("Test")], Encoding.Unicode, LineEndingStyle.CRLF);

        // act
        var result = _factory.Create(data);

        // assert
        Assert.AreEqual(Encoding.Unicode, result.Encoding);
    }

    [TestMethod]
    public void Create_WithDifferentLineEndingStyle_SetsLineEndingStyleCorrectly()
    {
        // arrange
        var data = new DocumentData([new("Test")], Encoding.UTF8, LineEndingStyle.CR);

        // act
        var result = _factory.Create(data);

        // assert
        Assert.AreEqual(LineEndingStyle.CR, result.LineEndingStyle);
    }

    [TestMethod]
    public void Create_NullData_ThrowsArgumentNullException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() => _factory.Create(null!));
    }

    [TestMethod]
    public void Create_NullLines_ThrowsArgumentNullException()
    {
        // arrange
        var data = new DocumentData(null!, Encoding.UTF8, LineEndingStyle.CRLF);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() => _factory.Create(data));
    }

    [TestMethod]
    public void Create_NullEncoding_ThrowsArgumentNullException()
    {
        // arrange
        var data = new DocumentData([new("Test")], null!, LineEndingStyle.CRLF);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() => _factory.Create(data));
    }

    [TestMethod]
    public void Create_NewDocument_IsModifiedIsFalse()
    {
        // arrange
        var data = new DocumentData([new("Test")], Encoding.UTF8, LineEndingStyle.CRLF);

        // act
        var result = _factory.Create(data);

        // assert
        Assert.IsFalse(result.IsModified);
    }
}