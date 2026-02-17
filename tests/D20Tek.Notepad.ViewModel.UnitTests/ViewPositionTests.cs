namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class ViewPositionTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var lineIndex = 5;
        var column = 10;

        // act
        var position = new ViewPosition(lineIndex, column);

        // assert
        Assert.AreEqual(lineIndex, position.LineIndex);
        Assert.AreEqual(column, position.Column);
    }

    [TestMethod]
    public void Constructor_WithZeroValues_SetsProperties()
    {
        // arrange & act
        var position = new ViewPosition(0, 0);

        // assert
        Assert.AreEqual(0, position.LineIndex);
        Assert.AreEqual(0, position.Column);
    }

    [TestMethod]
    public void Constructor_WithNegativeLineIndex_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => new ViewPosition(-1, 0));
    }

    [TestMethod]
    public void Constructor_WithNegativeColumn_ThrowsArgumentOutOfRangeException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            [ExcludeFromCodeCoverage] () => new ViewPosition(0, -1));
    }

    [TestMethod]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(5, 10);

        // act
        var result = pos1.Equals(pos2);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithDifferentLineIndex_ReturnsFalse()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(6, 10);

        // act
        var result = pos1.Equals(pos2);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentColumn_ReturnsFalse()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(5, 11);

        // act
        var result = pos1.Equals(pos2);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithObject_SameValues_ReturnsTrue()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        object pos2 = new ViewPosition(5, 10);

        // act
        var result = pos1.Equals(pos2);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithNull_ReturnsFalse()
    {
        // arrange
        var pos = new ViewPosition(5, 10);

        // act
        var result = pos.Equals(null);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // arrange
        var pos = new ViewPosition(5, 10);
        object other = "not a ViewPosition";

        // act
        var result = pos.Equals(other);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_WithSameValues_ReturnsSameHashCode()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(5, 10);

        // act & assert
        Assert.AreEqual(pos1.GetHashCode(), pos2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WithDifferentValues_ReturnsDifferentHashCode()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(6, 11);

        // act & assert
        Assert.AreNotEqual(pos1.GetHashCode(), pos2.GetHashCode());
    }

    [TestMethod]
    public void EqualityOperator_WithSameValues_ReturnsTrue()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(5, 10);

        // act
        var result = pos1 == pos2;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void EqualityOperator_WithDifferentValues_ReturnsFalse()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(6, 11);

        // act
        var result = pos1 == pos2;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void InequalityOperator_WithDifferentValues_ReturnsTrue()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(6, 11);

        // act
        var result = pos1 != pos2;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void InequalityOperator_WithSameValues_ReturnsFalse()
    {
        // arrange
        var pos1 = new ViewPosition(5, 10);
        var pos2 = new ViewPosition(5, 10);

        // act
        var result = pos1 != pos2;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        // arrange
        var position = new ViewPosition(5, 10);

        // act
        var result = position.ToString();

        // assert
        Assert.AreEqual("(5, 10)", result);
    }

    [TestMethod]
    public void ToString_WithZeroValues_ReturnsFormattedString()
    {
        // arrange
        var position = new ViewPosition(0, 0);

        // act
        var result = position.ToString();

        // assert
        Assert.AreEqual("(0, 0)", result);
    }
}
