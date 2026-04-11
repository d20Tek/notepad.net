using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Document;

[TestClass]
public class DocumentTests
{
    [TestMethod]
    public void Constructor_WithLines_SetsLinesProperty()
    {
        // arrange
        var lines = new[] { new TextLine("Hello"), new TextLine("World") };

        // act
        var doc = new Doc.Document(lines);

        // assert
        Assert.AreEqual(2, doc.LineCount);
        Assert.AreEqual("Hello", doc.Lines[0].Content);
        Assert.AreEqual("World", doc.Lines[1].Content);
    }

    [TestMethod]
    public void Constructor_WithLines_SetsDefaultEncodingAndLineEnding()
    {
        // arrange
        var lines = new[] { new TextLine("Test") };

        // act
        var doc = new Doc.Document(lines);

        // assert
        Assert.AreEqual(Encoding.UTF8, doc.Encoding);
        Assert.AreEqual(LineEndingStyle.CRLF, doc.LineEndingStyle);
    }

    [TestMethod]
    public void Constructor_WithEncodingAndLineEnding_SetsProperties()
    {
        // arrange
        var lines = new[] { new TextLine("Test") };
        var encoding = Encoding.ASCII;
        var lineEnding = LineEndingStyle.LF;

        // act
        var doc = new Doc.Document(lines, encoding, lineEnding);

        // assert
        Assert.AreEqual(encoding, doc.Encoding);
        Assert.AreEqual(lineEnding, doc.LineEndingStyle);
    }

    [TestMethod]
    public void Constructor_EmptyLines_EnsuresSingleEmptyLine()
    {
        // arrange
        var lines = Array.Empty<TextLine>();

        // act
        var doc = new Doc.Document(lines);

        // assert
        Assert.AreEqual(1, doc.LineCount);
        Assert.AreEqual(string.Empty, doc.Lines[0].Content);
    }

    [TestMethod]
    public void Constructor_NewDocument_IsModifiedIsFalse()
    {
        // arrange
        var lines = new[] { new TextLine("Test") };

        // act
        var doc = new Doc.Document(lines);

        // assert
        Assert.IsFalse(doc.IsModified);
    }

    [TestMethod]
    public void ReplaceLines_ValidRange_ReplacesLines()
    {
        // arrange
        var doc = new Doc.Document([new("Line1"), new("Line2"), new("Line3")]);
        var newLines = new[] { new TextLine("NewLine") };

        // act
        doc.ReplaceLines(1, 1, newLines);

        // assert
        Assert.AreEqual(3, doc.LineCount);
        Assert.AreEqual("Line1", doc.Lines[0].Content);
        Assert.AreEqual("NewLine", doc.Lines[1].Content);
        Assert.AreEqual("Line3", doc.Lines[2].Content);
    }

    [TestMethod]
    public void ReplaceLines_ValidRange_SetsIsModifiedTrue()
    {
        // arrange
        var doc = new Doc.Document([new("Line1"), new("Line2")]);
        var newLines = new[] { new TextLine("NewLine") };

        // act
        doc.ReplaceLines(0, 1, newLines);

        // assert
        Assert.IsTrue(doc.IsModified);
    }

    [TestMethod]
    public void ReplaceLines_InsertLines_IncreasesLineCount()
    {
        // arrange
        var doc = new Doc.Document([new("Line1"), new("Line2")]);
        var newLines = new[] { new TextLine("A"), new TextLine("B"), new TextLine("C") };

        // act
        doc.ReplaceLines(1, 1, newLines);

        // assert
        Assert.AreEqual(4, doc.LineCount);
        Assert.AreEqual("A", doc.Lines[1].Content);
        Assert.AreEqual("B", doc.Lines[2].Content);
        Assert.AreEqual("C", doc.Lines[3].Content);
    }

    [TestMethod]
    public void ReplaceLines_RemoveAllLines_EnsuresSingleEmptyLine()
    {
        // arrange
        var doc = new Doc.Document([new("Line1"), new("Line2")]);

        // act
        doc.ReplaceLines(0, 2, []);

        // assert
        Assert.AreEqual(1, doc.LineCount);
        Assert.AreEqual(string.Empty, doc.Lines[0].Content);
    }

    [TestMethod]
    public void SetEncoding_ValidEncoding_UpdatesEncoding()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);
        var newEncoding = Encoding.Unicode;

        // act
        doc.SetEncoding(newEncoding);

        // assert
        Assert.AreEqual(newEncoding, doc.Encoding);
    }

    [TestMethod]
    public void SetEncoding_ValidEncoding_SetsIsModifiedTrue()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act
        doc.SetEncoding(Encoding.ASCII);

        // assert
        Assert.IsTrue(doc.IsModified);
    }

    [TestMethod]
    public void SetLineEndingStyle_ValidStyle_UpdatesLineEndingStyle()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);
        var newStyle = LineEndingStyle.LF;

        // act
        doc.SetLineEndingStyle(newStyle);

        // assert
        Assert.AreEqual(newStyle, doc.LineEndingStyle);
    }

    [TestMethod]
    public void SetLineEndingStyle_ValidStyle_SetsIsModifiedTrue()
    {
        // arrange
        var doc = new Doc.Document([new("Test")]);

        // act
        doc.SetLineEndingStyle(LineEndingStyle.CR);

        // assert
        Assert.IsTrue(doc.IsModified);
    }

    [TestMethod]
    public void LineCount_ReturnsCorrectCount()
    {
        // arrange
        var doc = new Doc.Document([new("A"), new("B"), new("C"), new("D")]);

        // act
        var result = doc.LineCount;

        // assert
        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void TotalCharacterCount_Constructor_ReturnsSumPlusLineEndings()
    {
        // arrange - "Hello" (5) + "World" (5) + 1 line ending = 11
        var doc = new Doc.Document([new("Hello"), new("World")]);

        // act
        var result = doc.TotalCharacterCount;

        // assert
        Assert.AreEqual(11, result);
    }

    [TestMethod]
    public void TotalCharacterCount_EmptyDocument_ReturnsZero()
    {
        // arrange
        var doc = new Doc.Document([]);

        // act
        var result = doc.TotalCharacterCount;

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void TotalCharacterCount_AfterInsertLine_UpdatesIncrementally()
    {
        // arrange - "AB" (2) = 2 total
        var doc = new Doc.Document([new("AB")]);
        Assert.AreEqual(2, doc.TotalCharacterCount);

        // act - replace "AB" with "AB" + "CD" → 2 + 2 + 1 line ending = 5
        doc.ReplaceLines(0, 1, [new("AB"), new("CD")]);

        // assert
        Assert.AreEqual(5, doc.TotalCharacterCount);
    }

    [TestMethod]
    public void TotalCharacterCount_AfterRemoveLine_UpdatesIncrementally()
    {
        // arrange - "Hello" + "World" + "End" = 5+5+3 + 2 line endings = 15
        var doc = new Doc.Document([new("Hello"), new("World"), new("End")]);
        Assert.AreEqual(15, doc.TotalCharacterCount);

        // act - remove middle line → "Hello" + "End" = 5+3 + 1 line ending = 9
        doc.ReplaceLines(1, 1, []);

        // assert
        Assert.AreEqual(9, doc.TotalCharacterCount);
    }

    [TestMethod]
    public void TotalCharacterCount_AfterReplaceWithDifferentLength_UpdatesIncrementally()
    {
        // arrange - "ABC" (3) = 3 total
        var doc = new Doc.Document([new("ABC")]);

        // act - replace with "ABCDEF" (6) = 6 total
        doc.ReplaceLines(0, 1, [new("ABCDEF")]);

        // assert
        Assert.AreEqual(6, doc.TotalCharacterCount);
    }

    [TestMethod]
    public void TotalCharacterCount_AfterRemoveAll_ReturnsZero()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);

        // act
        doc.ReplaceLines(0, 2, []);

        // assert
        Assert.AreEqual(0, doc.TotalCharacterCount);
    }

    [TestMethod]
    public void MaxLineLength_Constructor_ReturnsLongestLineLength()
    {
        // arrange
        var doc = new Doc.Document([new("Hi"), new("Hello"), new("Hey")]);

        // act
        var result = doc.MaxLineLength;

        // assert
        Assert.AreEqual(5, result);
    }

    [TestMethod]
    public void MaxLineLength_EmptyDocument_ReturnsZero()
    {
        // arrange
        var doc = new Doc.Document([]);

        // act
        var result = doc.MaxLineLength;

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void MaxLineLength_AfterInsertLongerLine_UpdatesMax()
    {
        // arrange - max is 3 ("ABC")
        var doc = new Doc.Document([new("ABC")]);
        Assert.AreEqual(3, doc.MaxLineLength);

        // act - replace with longer line
        doc.ReplaceLines(0, 1, [new("ABCDEF")]);

        // assert
        Assert.AreEqual(6, doc.MaxLineLength);
    }

    [TestMethod]
    public void MaxLineLength_AfterRemoveLongestLine_Recomputes()
    {
        // arrange - max is 10 ("LongestLin")
        var doc = new Doc.Document([new("Hi"), new("LongestLin"), new("Hey")]);
        Assert.AreEqual(10, doc.MaxLineLength);

        // act - remove the longest line
        doc.ReplaceLines(1, 1, []);

        // assert - max is now 3 ("Hey")
        Assert.AreEqual(3, doc.MaxLineLength);
    }

    [TestMethod]
    public void MaxLineLength_AfterShortenNonMaxLine_KeepsMax()
    {
        // arrange
        var doc = new Doc.Document([new("ABCDE"), new("XY")]);
        Assert.AreEqual(5, doc.MaxLineLength);

        // act - shorten the short line further
        doc.ReplaceLines(1, 1, [new("X")]);

        // assert - max unchanged
        Assert.AreEqual(5, doc.MaxLineLength);
    }

    [TestMethod]
    public void MaxLineLength_AfterRemoveAll_ReturnsZero()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);

        // act
        doc.ReplaceLines(0, 2, []);

        // assert
        Assert.AreEqual(0, doc.MaxLineLength);
    }
}
