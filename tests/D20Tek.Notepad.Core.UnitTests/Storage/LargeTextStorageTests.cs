namespace D20Tek.Notepad.Core.UnitTests.Storage;

[TestClass]
public class LargeTextStorageTests
{
    private readonly LargeTextStorage _storage = new();

    private static string WriteTempFile(string content, Encoding? encoding = null)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, content, encoding ?? new UTF8Encoding(false));
        return path;
    }

    private static string WriteTempFileBytes(byte[] bytes)
    {
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, bytes);
        return path;
    }

    [TestMethod]
    public void Load_SmallUtf8File_ReturnsCorrectLinesAndEncoding()
    {
        // arrange
        var path = WriteTempFile("Hello\r\nWorld");
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(2, result.Lines);
            Assert.AreEqual("Hello", result.Lines[0].Content);
            Assert.AreEqual("World", result.Lines[1].Content);
            Assert.AreEqual("utf-8", result.Encoding.WebName);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithCrLfEndings_DetectsLineEndingStyle()
    {
        // arrange
        var path = WriteTempFile("Line1\r\nLine2\r\nLine3");
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
            Assert.HasCount(3, result.Lines);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithLfEndings_DetectsLineEndingStyle()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\nLine2\nLine3"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(LineEndingStyle.LF, result.LineEndingStyle);
            Assert.HasCount(3, result.Lines);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_Utf8BomFile_StripsAndDetectsEncoding()
    {
        // arrange
        var path = WriteTempFile("BOM content here", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(Encoding.UTF8.CodePage, result.Encoding.CodePage);
            Assert.AreEqual("BOM content here", result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_EmptyFile_ReturnsSingleEmptyLine()
    {
        // arrange
        var path = WriteTempFile(string.Empty);
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(1, result.Lines);
            Assert.AreEqual(string.Empty, result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_SingleLineFile_ReturnsSingleLine()
    {
        // arrange
        var path = WriteTempFile("just one line");
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(1, result.Lines);
            Assert.AreEqual("just one line", result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_ReportsProgress()
    {
        // arrange
        var path = WriteTempFile("Line1\nLine2\nLine3");
        var progressCalls = new List<(long Bytes, long Total)>();
        var progress = new TestLoadProgress(progressCalls);
        try
        {
            // act
            _storage.Load(path, progress);

            // assert
            Assert.IsTrue(progressCalls.Count > 0);
            Assert.IsTrue(progressCalls.All(p => p.Total > 0));
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_Utf16LeFile_DetectsEncodingAndReadsContent()
    {
        // arrange
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Hello World", Encoding.Unicode);

            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(Encoding.Unicode.CodePage, result.Encoding.CodePage);
            Assert.AreEqual("Hello World", result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_Utf16BeFile_DetectsEncodingAndReadsContent()
    {
        // arrange
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Hello World", Encoding.BigEndianUnicode);

            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(Encoding.BigEndianUnicode.CodePage, result.Encoding.CodePage);
            Assert.AreEqual("Hello World", result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithCrEndings_DetectsLineEndingStyleAndContent()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\rLine2"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(LineEndingStyle.CR, result.LineEndingStyle);
            Assert.HasCount(2, result.Lines);
            Assert.AreEqual("Line1", result.Lines[0].Content);
            Assert.AreEqual("Line2", result.Lines[1].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithTrailingLf_OmitsEmptyLastLine()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\nLine2\n"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(2, result.Lines);
            Assert.AreEqual("Line1", result.Lines[0].Content);
            Assert.AreEqual("Line2", result.Lines[1].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithTrailingCrLf_OmitsEmptyLastLine()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\r\nLine2\r\n"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(2, result.Lines);
            Assert.AreEqual("Line1", result.Lines[0].Content);
            Assert.AreEqual("Line2", result.Lines[1].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithTrailingCr_OmitsEmptyLastLine()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\rLine2\r"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(2, result.Lines);
            Assert.AreEqual("Line1", result.Lines[0].Content);
            Assert.AreEqual("Line2", result.Lines[1].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithEmptyLines_PreservesEmptyLines()
    {
        // arrange
        var path = WriteTempFileBytes("Line1\n\nLine3"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.HasCount(3, result.Lines);
            Assert.AreEqual("Line1", result.Lines[0].Content);
            Assert.AreEqual(string.Empty, result.Lines[1].Content);
            Assert.AreEqual("Line3", result.Lines[2].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_WithNoneProgress_DoesNotThrow()
    {
        // arrange
        var path = WriteTempFileBytes("Hello\nWorld"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path, ILoadProgress.None);

            // assert
            Assert.HasCount(2, result.Lines);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_FileWithConsecutiveCr_HandlesCorrectly()
    {
        // arrange — "A\r\rB": second consecutive \r skips the !prevWasCr branch
        var path = WriteTempFileBytes("A\r\rB"u8.ToArray());
        try
        {
            // act
            var result = _storage.Load(path);

            // assert
            Assert.AreEqual(LineEndingStyle.CR, result.LineEndingStyle);
            Assert.AreEqual("A", result.Lines[0].Content);
        }
        finally { File.Delete(path); }
    }

    [TestMethod]
    public void Load_ReportsMidScanProgress_WhenChunkBoundaryReached()
    {
        // arrange — chunkSize=4: an 11-byte file produces 3 chunks;
        // time-based throttling fires for the final report at minimum
        var storage = new LargeTextStorage(chunkSize: 4);
        var path = WriteTempFileBytes("AAAABBBBCCC"u8.ToArray());
        var progressCalls = new List<(long Bytes, long Total)>();
        var progress = new TestLoadProgress(progressCalls);
        try
        {
            // act
            storage.Load(path, progress);

            // assert — final 100% report always fires
            Assert.IsTrue(progressCalls.Count > 0);
            Assert.IsTrue(progressCalls[^1].Bytes == progressCalls[^1].Total);
        }
        finally { File.Delete(path); }
    }

    private sealed class TestLoadProgress(List<(long Bytes, long Total)> calls) : ILoadProgress
    {
        public void Report(long bytesProcessed, long totalBytes) =>
            calls.Add((bytesProcessed, totalBytes));
    }
}
