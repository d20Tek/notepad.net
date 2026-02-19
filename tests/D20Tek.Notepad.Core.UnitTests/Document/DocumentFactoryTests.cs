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

    // Load tests
    [TestMethod]
    public void Load_WithValidFile_ReturnsDocumentWithCorrectContent()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Hello\r\nWorld", Encoding.UTF8);

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(2, result.Lines.Count);
            Assert.AreEqual("Hello", result.Lines[0].Content);
            Assert.AreEqual("World", result.Lines[1].Content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithSingleLineFile_ReturnsDocumentWithOneLine()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Single line content", Encoding.UTF8);

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(1, result.Lines.Count);
            Assert.AreEqual("Single line content", result.Lines[0].Content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithEmptyFile_ReturnsDocumentWithSingleEmptyLine()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, string.Empty, Encoding.UTF8);

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(1, result.Lines.Count);
            Assert.AreEqual(string.Empty, result.Lines[0].Content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithUtf8Encoding_DetectsEncodingCorrectly()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "UTF-8 content with BOM", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual("UTF-8 content with BOM", result.Lines[0].Content);
            Assert.AreEqual(Encoding.UTF8.CodePage, result.Encoding.CodePage);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithCrlfLineEndings_DetectsLineEndingStyle()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Line1\r\nLine2\r\nLine3", Encoding.UTF8);

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithLfLineEndings_DetectsLineEndingStyle()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllBytes(tempFile, "Line1\nLine2\nLine3"u8.ToArray());

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(LineEndingStyle.LF, result.LineEndingStyle);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Load_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");

        // act & assert
        Assert.ThrowsExactly<FileNotFoundException>(
            [ExcludeFromCodeCoverage] () => _factory.Load(nonExistentPath));
    }

    [TestMethod]
    public void Load_NewlyLoadedDocument_IsModifiedIsFalse()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "Test content", Encoding.UTF8);

            // act
            var result = _factory.Load(tempFile);

            // assert
            Assert.IsFalse(result.IsModified);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}