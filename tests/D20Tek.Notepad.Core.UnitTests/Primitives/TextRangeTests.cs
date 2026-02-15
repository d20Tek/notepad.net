namespace D20Tek.Notepad.Core.UnitTests.Primitives;

[TestClass]
public class TextRangeTests
{
    [TestMethod]
    public void Constructor_SetsStartAndEnd()
    {
        // arrange
        var start = new TextPosition(1, 5);
        var end = new TextPosition(3, 10);

        // act
        var range = new TextRange(start, end);

        // assert
        Assert.AreEqual(start, range.Start);
        Assert.AreEqual(end, range.End);
    }

    [TestMethod]
    public void WithUpdate_ReturnsNewValue()
    {
        // arrange
        var start = new TextPosition(1, 5);
        var end = new TextPosition(3, 10);
        var range = new TextRange(new(0, 0), new(0, 0));

        // act
        var result = range with { Start = start, End = end };

        // assert
        Assert.AreEqual(start, result.Start);
        Assert.AreEqual(end, result.End);
    }

    [TestMethod]
    public void Normalized_WhenStartBeforeEnd_ReturnsSameRange()
    {
        // arrange
        var start = new TextPosition(1, 5);
        var end = new TextPosition(3, 10);
        var range = new TextRange(start, end);

        // act
        var result = range.Normalized();

        // assert
        Assert.AreEqual(start, result.Start);
        Assert.AreEqual(end, result.End);
    }

    [TestMethod]
    public void Normalized_WhenEndBeforeStart_SwapsStartAndEnd()
    {
        // arrange
        var start = new TextPosition(5, 10);
        var end = new TextPosition(2, 3);
        var range = new TextRange(start, end);

        // act
        var result = range.Normalized();

        // assert
        Assert.AreEqual(end, result.Start);
        Assert.AreEqual(start, result.End);
    }

    [TestMethod]
    public void Normalized_WhenStartEqualsEnd_ReturnsSameRange()
    {
        // arrange
        var position = new TextPosition(2, 5);
        var range = new TextRange(position, position);

        // act
        var result = range.Normalized();

        // assert
        Assert.AreEqual(position, result.Start);
        Assert.AreEqual(position, result.End);
    }

    [TestMethod]
    public void Contains_PositionWithinRange_ReturnsTrue()
    {
        // arrange
        var range = new TextRange(new TextPosition(1, 0), new TextPosition(3, 10));
        var position = new TextPosition(2, 5);

        // act
        var result = range.Contains(position);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Contains_PositionAtStart_ReturnsTrue()
    {
        // arrange
        var start = new TextPosition(1, 5);
        var range = new TextRange(start, new TextPosition(3, 10));

        // act
        var result = range.Contains(start);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Contains_PositionAtEnd_ReturnsTrue()
    {
        // arrange
        var end = new TextPosition(3, 10);
        var range = new TextRange(new TextPosition(1, 5), end);

        // act
        var result = range.Contains(end);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Contains_PositionBeforeRange_ReturnsFalse()
    {
        // arrange
        var range = new TextRange(new TextPosition(2, 5), new TextPosition(4, 10));
        var position = new TextPosition(1, 0);

        // act
        var result = range.Contains(position);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Contains_PositionAfterRange_ReturnsFalse()
    {
        // arrange
        var range = new TextRange(new TextPosition(2, 5), new TextPosition(4, 10));
        var position = new TextPosition(5, 0);

        // act
        var result = range.Contains(position);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Contains_SameLineBefore_ReturnsFalse()
    {
        // arrange
        var range = new TextRange(new TextPosition(2, 5), new TextPosition(2, 10));
        var position = new TextPosition(2, 3);

        // act
        var result = range.Contains(position);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Contains_SameLineAfter_ReturnsFalse()
    {
        // arrange
        var range = new TextRange(new TextPosition(2, 5), new TextPosition(2, 10));
        var position = new TextPosition(2, 12);

        // act
        var result = range.Contains(position);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsEmpty_WhenStartEqualsEnd_ReturnsTrue()
    {
        // arrange
        var position = new TextPosition(2, 5);
        var range = new TextRange(position, position);

        // act
        var result = range.IsEmpty;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsEmpty_WhenStartDiffersFromEnd_ReturnsFalse()
    {
        // arrange
        var range = new TextRange(new TextPosition(1, 5), new TextPosition(1, 10));

        // act
        var result = range.IsEmpty;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var range1 = new TextRange(new TextPosition(1, 5), new TextPosition(3, 10));
        var range2 = new TextRange(new TextPosition(1, 5), new TextPosition(3, 10));

        // act
        var areEqual = range1 == range2;

        // assert
        Assert.AreEqual(range1, range2);
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var range1 = new TextRange(new TextPosition(1, 5), new TextPosition(3, 10));
        var range2 = new TextRange(new TextPosition(1, 5), new TextPosition(3, 11));

        // act
        var areNotEqual = range1 != range2;

        // assert
        Assert.AreNotEqual(range1, range2);
        Assert.IsTrue(areNotEqual);
    }
}
