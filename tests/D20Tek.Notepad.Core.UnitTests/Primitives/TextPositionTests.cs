using D20Tek.Notepad.Core.Primitives;

namespace D20Tek.Notepad.Core.UnitTests.Primitives;

[TestClass]
public class TextPositionTests
{
    [TestMethod]
    public void Constructor_SetsLineAndColumn()
    {
        // arrange
        var expectedLine = 5;
        var expectedColumn = 10;

        // act
        var position = new TextPosition(expectedLine, expectedColumn);

        // assert
        Assert.AreEqual(expectedLine, position.Line);
        Assert.AreEqual(expectedColumn, position.Column);
    }

    [TestMethod]
    public void WithUpdate_ReturnsNewResult()
    {
        // arrange
        var expectedLine = 5;
        var expectedColumn = 10;
        var position = new TextPosition(0, 0);

        // act
        var result = position with { Line = expectedLine, Column = expectedColumn };

        // assert
        Assert.AreEqual(expectedLine, result.Line);
        Assert.AreEqual(expectedColumn, result.Column);
    }

    [TestMethod]
    public void CompareTo_SameLine_ComparesColumn()
    {
        // arrange
        var position1 = new TextPosition(5, 3);
        var position2 = new TextPosition(5, 7);

        // act
        var result1 = position1.CompareTo(position2);
        var result2 = position2.CompareTo(position1);

        // assert
        Assert.IsLessThan(0, result1);
        Assert.IsGreaterThan(0, result2);
    }

    [TestMethod]
    public void CompareTo_DifferentLine_ComparesLine()
    {
        // arrange
        var position1 = new TextPosition(3, 10);
        var position2 = new TextPosition(5, 2);

        // act
        var result1 = position1.CompareTo(position2);
        var result2 = position2.CompareTo(position1);

        // assert
        Assert.IsLessThan(0, result1);
        Assert.IsGreaterThan(0, result2);
    }

    [TestMethod]
    public void CompareTo_EqualPositions_ReturnsZero()
    {
        // arrange
        var position1 = new TextPosition(5, 10);
        var position2 = new TextPosition(5, 10);

        // act
        var result = position1.CompareTo(position2);

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        // arrange
        var position = new TextPosition(3, 7);

        // act
        var result = position.ToString();

        // assert
        Assert.AreEqual("(3, 7)", result);
    }

    [TestMethod]
    public void Min_ReturnsSmaller_WhenFirstIsSmaller()
    {
        // arrange
        var smaller = new TextPosition(2, 5);
        var larger = new TextPosition(3, 1);

        // act
        var result = TextPosition.Min(smaller, larger);

        // assert
        Assert.AreEqual(smaller, result);
    }

    [TestMethod]
    public void Min_ReturnsSmaller_WhenSecondIsSmaller()
    {
        // arrange
        var larger = new TextPosition(3, 1);
        var smaller = new TextPosition(2, 5);

        // act
        var result = TextPosition.Min(larger, smaller);

        // assert
        Assert.AreEqual(smaller, result);
    }

    [TestMethod]
    public void Max_ReturnsLarger_WhenFirstIsLarger()
    {
        // arrange
        var larger = new TextPosition(5, 10);
        var smaller = new TextPosition(5, 3);

        // act
        var result = TextPosition.Max(larger, smaller);

        // assert
        Assert.AreEqual(larger, result);
    }

    [TestMethod]
    public void Max_ReturnsLarger_WhenSecondIsLarger()
    {
        // arrange
        var smaller = new TextPosition(5, 3);
        var larger = new TextPosition(5, 10);

        // act
        var result = TextPosition.Max(smaller, larger);

        // assert
        Assert.AreEqual(larger, result);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var position1 = new TextPosition(5, 10);
        var position2 = new TextPosition(5, 10);

        // act
        var areEqual = position1 == position2;

        // assert
        Assert.AreEqual(position1, position2);
        Assert.IsTrue(areEqual);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var position1 = new TextPosition(5, 10);
        var position2 = new TextPosition(5, 11);

        // act
        var areNotEqual = position1 != position2;

        // assert
        Assert.AreNotEqual(position1, position2);
        Assert.IsTrue(areNotEqual);
    }
}
