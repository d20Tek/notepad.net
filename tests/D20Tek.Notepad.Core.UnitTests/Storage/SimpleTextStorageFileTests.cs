namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class SimpleTextStorageFileTests
{
    private readonly SimpleTextStorage _storage = new();
    private const string _testFilesPath = "test-files";

    [TestMethod]
    [DataRow("utf8-with-bom.txt", "UTF-8")]
    [DataRow("utf8-no-bom.txt", "UTF-8")]
    [DataRow("utf16-le.txt", "UTF-16")]
    [DataRow("utf16-be.txt", "UTF-16BE")]
    public void Load_RealFile_DetectsEncodingCorrectly(string filename, string expectedEncoding)
    {
        // arrange
        var path = Path.Combine(_testFilesPath, filename);
        using var stream = File.OpenRead(path);

        // act
        var result = _storage.Load(stream);

        // assert
        Assert.AreEqual(expectedEncoding, result.Encoding.WebName, true);
    }
}