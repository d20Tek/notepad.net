using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class SimpleTextStorageTests
{
    private readonly SimpleTextStorage _storage = new();

    // Load Tests
    [TestMethod]
    public void Load_SimpleText_ReturnsCorrectLines()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("Hello\r\nWorld");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(2, result.Lines);
        Assert.AreEqual("Hello", result.Lines[0].Content);
        Assert.AreEqual("World", result.Lines[1].Content);
    }

    [TestMethod]
    public void Load_EmptyStream_ReturnsSingleEmptyLine()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream(string.Empty);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(1, result.Lines);
        Assert.AreEqual(string.Empty, result.Lines[0].Content);
    }

    [TestMethod]
    public void Load_CrlfLineEndings_DetectsCrlf()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("Line1\r\nLine2\r\nLine3");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
    }

    [TestMethod]
    public void Load_LfLineEndings_DetectsLf()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("Line1\nLine2\nLine3");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(LineEndingStyle.LF, result.LineEndingStyle);
    }

    [TestMethod]
    public void Load_CrLineEndings_DetectsCr()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("Line1\rLine2\rLine3");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(LineEndingStyle.CR, result.LineEndingStyle);
    }

    [TestMethod]
    public void Load_Utf8WithBom_DetectsUtf8Encoding()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStreamWithBom("Hello", Encoding.UTF8);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(Encoding.UTF8, result.Encoding);
    }

    [TestMethod]
    public void Load_Utf16LeWithBom_DetectsUnicodeEncoding()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStreamWithBom("Hello", Encoding.Unicode);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(Encoding.Unicode, result.Encoding);
    }

    // Save Tests
    [TestMethod]
    public void Save_SimpleDocument_WritesCorrectContent()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        using var stream = new MemoryStream();

        // act
        _storage.Save(stream, doc);

        // assert
        var result = MemoryStreamFactory.ReadStream(stream);
        Assert.AreEqual("Hello\r\nWorld", result);
    }

    [TestMethod]
    public void Save_DocumentWithLfLineEnding_UsesLf()
    {
        // arrange
        var doc = new Doc.Document(
            [new("Line1"), new("Line2")],
            Encoding.UTF8,
            LineEndingStyle.LF);
        using var stream = new MemoryStream();

        // act
        _storage.Save(stream, doc);

        // assert
        var result = MemoryStreamFactory.ReadStream(stream);
        Assert.AreEqual("Line1\nLine2", result);
    }

    [TestMethod]
    public void Save_EmptyDocument_WritesSingleEmptyLine()
    {
        // arrange
        var doc = new Doc.Document([new("")]);
        using var stream = new MemoryStream();

        // act
        _storage.Save(stream, doc);

        // assert
        var result = MemoryStreamFactory.ReadStream(stream);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void LoadAndSave_RoundTrip_PreservesContent()
    {
        // arrange
        var originalText = "Line1\r\nLine2\r\nLine3";
        using var loadStream = MemoryStreamFactory.CreateStream(originalText);
        var documentData = _storage.Load(loadStream);
        var doc = new Doc.Document(documentData.Lines, documentData.Encoding, documentData.LineEndingStyle);
        using var saveStream = new MemoryStream();

        // act
        _storage.Save(saveStream, doc);

        // assert
        var result = MemoryStreamFactory.ReadStream(saveStream);
        Assert.AreEqual(originalText, result);
    }
}
