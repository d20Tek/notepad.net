using D20Tek.Notepad.Core.Primitives;
using System.Text;
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
}
