using Doc = D20Tek.Notepad.Core.Document;

namespace D20Tek.Notepad.Core.UnitTests.Document;

[TestClass]
public class DocumentPositionExtensionsTests
{
    [TestMethod]
    public void Normalize_ValidPosition_ReturnsSamePosition()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World"), new("Test")]);
        var position = new TextPosition(1, 3);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(new TextPosition(1, 3), result);
    }

    [TestMethod]
    public void Normalize_NegativeLine_ClampsToZero()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var position = new TextPosition(-5, 2);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(0, result.Line);
    }

    [TestMethod]
    public void Normalize_LineBeyondCount_ClampsToLastLine()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World"), new("Test")]);
        var position = new TextPosition(10, 2);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(2, result.Line);
    }

    [TestMethod]
    public void Normalize_NegativeColumn_ClampsToZero()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var position = new TextPosition(0, -3);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(0, result.Column);
    }

    [TestMethod]
    public void Normalize_ColumnBeyondLineLength_ClampsToLineEnd()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new("World")]);
        var position = new TextPosition(0, 100);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void Normalize_BothOutOfBounds_ClampsBothValues()
    {
        // arrange
        var doc = new Doc.Document([new("Hi"), new("There")]);
        var position = new TextPosition(50, 100);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(1, result.Line);
        Assert.AreEqual(5, result.Column);
    }

    [TestMethod]
    public void Normalize_EmptyLine_ClampsColumnToZero()
    {
        // arrange
        var doc = new Doc.Document([new("Hello"), new(""), new("World")]);
        var position = new TextPosition(1, 5);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(1, result.Line);
        Assert.AreEqual(0, result.Column);
    }

    [TestMethod]
    public void Normalize_ColumnAtExactLineLength_ReturnsValidPosition()
    {
        // arrange
        var doc = new Doc.Document([new("Hello")]);
        var position = new TextPosition(0, 5);

        // act
        var result = doc.Normalize(position);

        // assert
        Assert.AreEqual(new TextPosition(0, 5), result);
    }
}
