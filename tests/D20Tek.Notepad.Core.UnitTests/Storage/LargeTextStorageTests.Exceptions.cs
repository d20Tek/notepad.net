namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
[ExcludeFromCodeCoverage]
public class LargeTextStorageExceptionTests
{
    private readonly LargeTextStorage _storage = new();

    [TestMethod]
    public void Load_NullFilePath_ThrowsArgumentNullException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _storage.Load(null!));
    }

    [TestMethod]
    public void Load_WhitespaceFilePath_ThrowsArgumentException()
    {
        // arrange, act & assert
        Assert.ThrowsExactly<ArgumentException>(() => _storage.Load("   "));
    }

    [TestMethod]
    public void Load_NonExistentFile_ThrowsFileNotFoundException()
    {
        // arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");

        // act & assert
        Assert.ThrowsExactly<FileNotFoundException>(() => _storage.Load(nonExistentPath));
    }

    [TestMethod]
    public void Load_CancellationRequested_ThrowsOperationCanceledException()
    {
        // arrange
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, "Line1\nLine2\nLine3"u8.ToArray());
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            // act & assert — cancellation is checked at the top of BuildLineIndex's while loop
            Assert.ThrowsExactly<OperationCanceledException>(() => _storage.Load(path, null, cts.Token));
        }
        finally { File.Delete(path); }
    }
}
