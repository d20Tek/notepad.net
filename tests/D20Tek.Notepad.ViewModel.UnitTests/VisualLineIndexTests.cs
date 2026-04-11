namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class VisualLineIndexTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void Rebuild_WithShortLines_SetsCorrectTotal()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World", "Test"]);
        var index = new VisualLineIndex();

        // act
        index.Rebuild(doc, 80);

        // assert
        Assert.AreEqual(3, index.TotalVisualLineCount);
    }

    [TestMethod]
    public void Rebuild_WithWrappedLines_SetsCorrectTotal()
    {
        // arrange
        var doc = CreateDocument(["One Two Three Four", "Short", "ABCDEFGHIJ"]);
        var index = new VisualLineIndex();

        // act
        index.Rebuild(doc, 8);

        // assert
        Assert.AreEqual(6, index.TotalVisualLineCount);
    }

    [TestMethod]
    public void NeedsRebuild_WhenLineCountChanged_ReturnsTrue()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        var result = index.NeedsRebuild(3, 80);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void NeedsRebuild_WhenWidthChanged_ReturnsTrue()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        var result = index.NeedsRebuild(2, 100);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void NeedsRebuild_WhenNothingChanged_ReturnsFalse()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        var result = index.NeedsRebuild(2, 80);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void UpdateLine_WithChangedSegmentCount_UpdatesTotal()
    {
        // arrange
        var doc = CreateDocument(["Short", "Short"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 20);
        Assert.AreEqual(2, index.TotalVisualLineCount);

        var updatedDoc = CreateDocument(["This is a much longer line that will wrap", "Short"]);

        // act
        index.UpdateLine(updatedDoc, 0);

        // assert
        Assert.AreEqual(4, index.TotalVisualLineCount);
    }

    [TestMethod]
    public void UpdateLine_WithUnchangedSegmentCount_KeepsTotal()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        var updatedDoc = CreateDocument(["Howdy", "World"]);

        // act
        index.UpdateLine(updatedDoc, 0);

        // assert
        Assert.AreEqual(2, index.TotalVisualLineCount);
    }

    [TestMethod]
    public void UpdateLine_WithInvalidIndex_DoesNothing()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        index.UpdateLine(doc, -1);
        index.UpdateLine(doc, 5);

        // assert
        Assert.AreEqual(2, index.TotalVisualLineCount);
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithNoWrapping_ReturnsLineIndex()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World", "Test"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act & assert
        Assert.AreEqual(0, index.GetVisualLineIndexForDocumentLine(0));
        Assert.AreEqual(1, index.GetVisualLineIndexForDocumentLine(1));
        Assert.AreEqual(2, index.GetVisualLineIndexForDocumentLine(2));
    }

    [TestMethod]
    public void GetVisualLineIndexForDocumentLine_WithWrapping_ReturnsCorrectIndex()
    {
        // arrange
        var doc = CreateDocument(["One Two Three Four", "Short", "End"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 8);

        // act & assert
        Assert.AreEqual(0, index.GetVisualLineIndexForDocumentLine(0));
        Assert.AreEqual(3, index.GetVisualLineIndexForDocumentLine(1));
        Assert.AreEqual(4, index.GetVisualLineIndexForDocumentLine(2));
    }

    [TestMethod]
    public void FindDocumentLineForVisualLine_WithNoWrapping_ReturnsDirectMapping()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World", "Test"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        var (docLine, offset) = index.FindDocumentLineForVisualLine(1);

        // assert
        Assert.AreEqual(1, docLine);
        Assert.AreEqual(0, offset);
    }

    [TestMethod]
    public void FindDocumentLineForVisualLine_WithWrapping_ReturnsCorrectDocLine()
    {
        // arrange
        var doc = CreateDocument(["One Two Three Four", "Short", "End"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 8);

        // act
        var (docLine, offset) = index.FindDocumentLineForVisualLine(3);

        // assert
        Assert.AreEqual(1, docLine);
        Assert.AreEqual(0, offset);
    }

    [TestMethod]
    public void FindDocumentLineForVisualLine_WithWrapping_ReturnsCorrectSegmentOffset()
    {
        // arrange
        var doc = CreateDocument(["One Two Three Four", "Short"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 8);

        // act
        var (docLine, offset) = index.FindDocumentLineForVisualLine(1);

        // assert
        Assert.AreEqual(0, docLine);
        Assert.AreEqual(1, offset);
    }

    [TestMethod]
    public void FindDocumentLineForVisualLine_BeyondEnd_ReturnsLastLine()
    {
        // arrange
        var doc = CreateDocument(["Hello", "World"]);
        var index = new VisualLineIndex();
        index.Rebuild(doc, 80);

        // act
        var (docLine, _) = index.FindDocumentLineForVisualLine(100);

        // assert
        Assert.AreEqual(1, docLine);
    }

    private static IDocument CreateDocument(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        return _docFactory.Create(new(textLines, System.Text.Encoding.UTF8, LineEndingStyle.CRLF));
    }
}
