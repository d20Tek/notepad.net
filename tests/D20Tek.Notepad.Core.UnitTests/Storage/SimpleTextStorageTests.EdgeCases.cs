namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class SimpleTextStorageEdgeCasesTests
{
    private readonly SimpleTextStorage _storage = new();

    [TestMethod]
    public void Load_TrailingNewline_PreservesEmptyLastLine()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("Line1\r\nLine2\r\n");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(3, result.Lines);
        Assert.AreEqual(string.Empty, result.Lines[2].Content);
    }

    [TestMethod]
    public void Load_MixedLineEndings_SplitsAllLines()
    {
        // arrange - mixed CRLF, LF, CR
        using var stream = MemoryStreamFactory.CreateStream("Line1\r\nLine2\nLine3\rLine4");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(4, result.Lines);
    }

    [TestMethod]
    public void Load_VeryLongLine_HandlesCorrectly()
    {
        // arrange
        var longLine = new string('x', 100_000);
        using var stream = MemoryStreamFactory.CreateStream(longLine);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(1, result.Lines);
        Assert.AreEqual(100_000, result.Lines[0].Content.Length);
    }

    [TestMethod]
    public void Load_OnlyNewlines_CreatesEmptyLines()
    {
        // arrange
        using var stream = MemoryStreamFactory.CreateStream("\r\n\r\n\r\n");

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.HasCount(4, result.Lines);
        Assert.IsTrue(result.Lines.All(l => l.Content == string.Empty));
    }

    [TestMethod]
    public void Load_Utf8WithoutBom_DefaultsToUtf8()
    {
        // arrange - UTF-8 bytes without BOM
        var bytes = Encoding.UTF8.GetBytes("Hello 世界");
        using var stream = new MemoryStream(bytes);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual("Hello 世界", result.Lines[0].Content);
    }
}