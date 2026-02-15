namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class EncodingDetectorTests
{
    [TestMethod]
    public void DetectEncoding_Utf8WithBom_ReturnsUtf8()
    {
        // arrange
        byte[] content = [0xEF, 0xBB, 0xBF, .. "Hello"u8];
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual(Encoding.UTF8, result.Encoding);
        Assert.AreEqual(3, result.PreambleLength);
    }

    [TestMethod]
    public void DetectEncoding_Utf16LittleEndianWithBom_ReturnsUnicode()
    {
        // arrange
        byte[] content = [0xFF, 0xFE, 0x48, 0x00]; // "H" in UTF-16 LE
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual(Encoding.Unicode, result.Encoding);
        Assert.AreEqual(2, result.PreambleLength);
    }

    [TestMethod]
    public void DetectEncoding_Utf16BigEndianWithBom_ReturnsBigEndianUnicode()
    {
        // arrange
        byte[] content = [0xFE, 0xFF, 0x00, 0x48]; // "H" in UTF-16 BE
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual(Encoding.BigEndianUnicode, result.Encoding);
        Assert.AreEqual(2, result.PreambleLength);
    }

    [TestMethod]
    public void DetectEncoding_NoBom_ReturnsUtf8WithoutBom()
    {
        // arrange
        byte[] content = "Hello World"u8.ToArray();
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual("utf-8", result.Encoding.WebName);
        Assert.AreEqual(0, result.PreambleLength);
        Assert.AreEqual(0, result.Encoding.Preamble.Length); // No BOM identifier
    }

    [TestMethod]
    public void DetectEncoding_EmptyStream_ReturnsUtf8WithoutBom()
    {
        // arrange
        using var stream = new MemoryStream([]);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual("utf-8", result.Encoding.WebName);
        Assert.AreEqual(0, result.PreambleLength);
    }

    [TestMethod]
    public void DetectEncoding_PartialBom_ReturnsUtf8WithoutBom()
    {
        // arrange - only first 2 bytes of UTF-8 BOM
        byte[] content = [0xEF, 0xBB];
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual("utf-8", result.Encoding.WebName);
        Assert.AreEqual(0, result.PreambleLength);
    }

    [TestMethod]
    public void DetectEncoding_SingleByte_ReturnsUtf8WithoutBom()
    {
        // arrange
        byte[] content = [0x41]; // 'A'
        using var stream = new MemoryStream(content);

        // act
        var result = stream.DetectEncoding();

        // assert
        Assert.AreEqual("utf-8", result.Encoding.WebName);
        Assert.AreEqual(0, result.PreambleLength);
    }
}
