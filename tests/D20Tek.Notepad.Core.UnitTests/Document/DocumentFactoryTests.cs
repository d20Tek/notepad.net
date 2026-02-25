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

    // Empty tests
    [TestMethod]
    public void Empty_ReturnsDocumentWithSingleEmptyLine()
    {
        // act
        var result = _factory.Empty;

        // assert
        Assert.AreEqual(1, result.Lines.Count);
        Assert.AreEqual(string.Empty, result.Lines[0].Content);
    }

    [TestMethod]
    public void Empty_ReturnsDocumentWithUtf8Encoding()
    {
        // act
        var result = _factory.Empty;

        // assert
        Assert.AreEqual(Encoding.UTF8.CodePage, result.Encoding.CodePage);
    }

    [TestMethod]
    public void Empty_ReturnsDocumentWithCrlfLineEnding()
    {
        // act
        var result = _factory.Empty;

        // assert
        Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
    }

    [TestMethod]
    public void Empty_ReturnsSameInstanceOnMultipleCalls()
    {
        // act
        var result1 = _factory.Empty;
        var result2 = _factory.Empty;

        // assert
        Assert.AreSame(result1, result2);
    }

    [TestMethod]
    public void Empty_IsModifiedIsFalse()
    {
        // act
        var result = _factory.Empty;

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

    // CreateEmpty tests
    [TestMethod]
    public void CreateEmpty_ReturnsDocumentWithSingleEmptyLine()
    {
        // act
        var result = DocumentFactory.CreateEmpty();

        // assert
        Assert.AreEqual(1, result.Lines.Count);
        Assert.AreEqual(string.Empty, result.Lines[0].Content);
    }

    [TestMethod]
    public void CreateEmpty_ReturnsNewInstanceEachTime()
    {
        // act
        var result1 = DocumentFactory.CreateEmpty();
        var result2 = DocumentFactory.CreateEmpty();

        // assert
        Assert.AreNotSame(result1, result2);
    }

    [TestMethod]
    public void CreateEmpty_ReturnsDocumentWithUtf8Encoding()
    {
        // act
        var result = DocumentFactory.CreateEmpty();

        // assert
        Assert.AreEqual(Encoding.UTF8.CodePage, result.Encoding.CodePage);
    }

    [TestMethod]
    public void CreateEmpty_ReturnsDocumentWithCrlfLineEnding()
    {
        // act
        var result = DocumentFactory.CreateEmpty();

        // assert
        Assert.AreEqual(LineEndingStyle.CRLF, result.LineEndingStyle);
    }

    // Save tests
    [TestMethod]
    public void Save_WithValidDocument_WritesContentToFile()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var lines = new List<TextLine> { new("Hello"), new("World") };
            var data = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);
            var doc = _factory.Create(data);

            // act
            _factory.Save(doc, tempFile);

            // assert
            var content = File.ReadAllText(tempFile);
            Assert.AreEqual("Hello\r\nWorld", content);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    [ExcludeFromCodeCoverage]
    public void Save_WithLfLineEnding_WritesCorrectLineEndings()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var lines = new List<TextLine> { new("Line1"), new("Line2") };
            var data = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.LF);
            var doc = _factory.Create(data);

            // act
            _factory.Save(doc, tempFile);

            // assert
            var bytes = File.ReadAllBytes(tempFile);
            var content = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(content.Contains("\n") && !content.Contains("\r\n"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Save_WithNullDocument_ThrowsArgumentNullException()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            // act & assert
            Assert.ThrowsExactly<ArgumentNullException>(
                [ExcludeFromCodeCoverage] () => _factory.Save(null!, tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void Save_WithNullFilePath_ThrowsArgumentException()
    {
        // arrange
        var doc = DocumentFactory.CreateEmpty();

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _factory.Save(doc, null!));
    }

    [TestMethod]
    public void Save_WithEmptyFilePath_ThrowsArgumentException()
    {
        // arrange
        var doc = DocumentFactory.CreateEmpty();

        // act & assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _factory.Save(doc, string.Empty));
    }

    [TestMethod]
    public void Save_WithWhitespaceFilePath_ThrowsArgumentException()
    {
        // arrange
        var doc = DocumentFactory.CreateEmpty();

        // act & assert
        Assert.ThrowsExactly<ArgumentException>(
            [ExcludeFromCodeCoverage] () => _factory.Save(doc, "   "));
    }

    [TestMethod]
    public void Save_ThenLoad_RoundTripsCorrectly()
    {
        // arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var lines = new List<TextLine> { new("Test"), new("Round"), new("Trip") };
            var data = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);
            var original = _factory.Create(data);

            // act
            _factory.Save(original, tempFile);
            var loaded = _factory.Load(tempFile);

            // assert
            Assert.AreEqual(original.Lines.Count, loaded.Lines.Count);
            for (int i = 0; i < original.Lines.Count; i++)
            {
                Assert.AreEqual(original.Lines[i].Content, loaded.Lines[i].Content);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}