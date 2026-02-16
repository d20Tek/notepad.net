using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
public class RangeEditingServiceTests
{
    [TestMethod]
    public void ReplaceRange_SingleLinePartialSelection_ReplacesText()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(0, 11));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "Universe");

        // assert
        Assert.AreEqual("Hello Universe", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 14), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_SingleLineEntireLine_ReplacesAllContent()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "World");

        // assert
        Assert.AreEqual("World", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_InsertAtCursor_InsertsText()
    {
        // arrange
        var doc = new Doc.Document([new("HelloWorld")]);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, " ");

        // assert
        Assert.AreEqual("Hello World", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 6), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_DeleteSelection_RemovesText()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 11));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "");

        // assert
        Assert.AreEqual("Hello", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 5), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_SpanningMultipleLines_ReplacesAndMerges()
    {
        // arrange
        var doc = new Doc.Document([new("First line"), new("Second line"), new("Third line")]);
        var range = new TextRange(new TextPosition(0, 6), new TextPosition(2, 5));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "and last");

        // assert
        Assert.AreEqual(1, doc.LineCount);
        Assert.AreEqual("First and last line", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 14), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_InsertMultipleLines_SplitsDocument()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var range = new TextRange(new TextPosition(0, 5), new TextPosition(0, 5));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "\nNew Line\n");

        // assert
        Assert.AreEqual(3, doc.LineCount);
        Assert.AreEqual("Hello", doc.Lines[0].Content);
        Assert.AreEqual("New Line", doc.Lines[1].Content);
        Assert.AreEqual(" World", doc.Lines[2].Content);
        Assert.AreEqual(new TextPosition(2, 0), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_DeleteMultipleLines_MergesRemainingContent()
    {
        // arrange
        var doc = new Doc.Document([new("Line 1"), new("Line 2"), new("Line 3")]);
        var range = new TextRange(new TextPosition(0, 4), new TextPosition(2, 4));

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "");

        // assert
        Assert.AreEqual(1, doc.LineCount);
        Assert.AreEqual("Line 3", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 4), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_ReversedRange_NormalizesAndReplaces()
    {
        // arrange
        var doc = new Doc.Document([new("Hello World")]);
        var range = new TextRange(new TextPosition(0, 11), new TextPosition(0, 6)); // reversed

        // act
        var newCaret = RangeEditingService.ReplaceRange(doc, range, "Universe");

        // assert
        Assert.AreEqual("Hello Universe", doc.Lines[0].Content);
        Assert.AreEqual(new TextPosition(0, 14), newCaret);
    }

    [TestMethod]
    public void ReplaceRange_NullDocument_ThrowsArgumentNullException()
    {
        // arrange
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() =>
            RangeEditingService.ReplaceRange(null!, range, "text"));
    }

    [TestMethod]
    public void ReplaceRange_NullReplacementText_ThrowsArgumentNullException()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);
        var range = new TextRange(new TextPosition(0, 0), new TextPosition(0, 5));

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage]() =>
            RangeEditingService.ReplaceRange(doc, range, null!));
    }
}
