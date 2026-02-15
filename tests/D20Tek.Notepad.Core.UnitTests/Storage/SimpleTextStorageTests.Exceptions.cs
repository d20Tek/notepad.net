using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
[ExcludeFromCodeCoverage]
public class SimpleTextStorageExcetionTests
{
    private readonly SimpleTextStorage _storage = new();

    [TestMethod]
    public void Load_NullStream_ThrowsArgumentNullException()
    {
        // arrange

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _storage.Load(null!));
    }

    [TestMethod]
    public void Save_NullStream_ThrowsArgumentNullException()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _storage.Save(null!, doc));
    }

    [TestMethod]
    public void Save_NullDocument_ThrowsArgumentNullException()
    {
        // arrange
        using var stream = new MemoryStream();

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _storage.Save(stream, null!));
    }
}
