namespace D20Tek.Notepad.Core.UnitTests.Document;

[TestClass]
public class DocumentDataTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var lines = new List<TextLine> { new("Hello"), new("World") };
        var encoding = Encoding.UTF8;
        var lineEnding = LineEndingStyle.CRLF;

        // act
        var data = new DocumentData(lines, encoding, lineEnding);

        // assert
        Assert.HasCount(2, data.Lines);
        Assert.AreEqual("Hello", data.Lines[0].Content);
        Assert.AreEqual("World", data.Lines[1].Content);
        Assert.AreEqual(encoding, data.Encoding);
        Assert.AreEqual(lineEnding, data.LineEndingStyle);
    }

    [TestMethod]
    public void With_ChangingAllProperties_CreatesNewInstanceWithAllUpdates()
    {
        // arrange
        var original = new DocumentData([new("Original")], Encoding.UTF8, LineEndingStyle.CRLF);
        var newLines = new List<TextLine> { new("New"), new("Lines") };

        // act
        var modified = original with
        {
            Lines = newLines,
            Encoding = Encoding.ASCII,
            LineEndingStyle = LineEndingStyle.LF
        };

        // assert
        Assert.AreNotSame(original, modified);
        Assert.HasCount(2, modified.Lines);
        Assert.AreEqual("New", modified.Lines[0].Content);
        Assert.AreEqual("Lines", modified.Lines[1].Content);
        Assert.AreEqual(Encoding.ASCII, modified.Encoding);
        Assert.AreEqual(LineEndingStyle.LF, modified.LineEndingStyle);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var lines = new List<TextLine> { new("Test") };
        var data1 = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);
        var data2 = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);

        // act
        var areEqual = data1 == data2;

        // assert
        Assert.IsTrue(areEqual);
        Assert.AreEqual(data1, data2);
    }

    [TestMethod]
    public void Equality_DifferentEncoding_AreNotEqual()
    {
        // arrange
        var lines = new List<TextLine> { new("Test") };
        var data1 = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);
        var data2 = new DocumentData(lines, Encoding.ASCII, LineEndingStyle.CRLF);

        // act
        var areNotEqual = data1 != data2;

        // assert
        Assert.IsTrue(areNotEqual);
        Assert.AreNotEqual(data1, data2);
    }

    [TestMethod]
    public void Equality_DifferentLineEndingStyle_AreNotEqual()
    {
        // arrange
        var lines = new List<TextLine> { new("Test") };
        var data1 = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.CRLF);
        var data2 = new DocumentData(lines, Encoding.UTF8, LineEndingStyle.LF);

        // act
        var areNotEqual = data1 != data2;

        // assert
        Assert.IsTrue(areNotEqual);
    }
}
