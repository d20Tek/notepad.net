namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class SelectionViewRangeTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var start = new ViewPosition(1, 5);
        var end = new ViewPosition(3, 10);

        // act
        var range = new SelectionViewRange(start, end);

        // assert
        Assert.AreEqual(start, range.Start);
        Assert.AreEqual(end, range.End);
    }

    [TestMethod]
    public void IsEmpty_WhenStartEqualsEnd_ReturnsTrue()
    {
        // arrange
        var position = new ViewPosition(1, 5);
        var range = new SelectionViewRange(position, position);

        // act
        var result = range.IsEmpty;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsEmpty_WhenStartDiffersFromEnd_ReturnsFalse()
    {
        // arrange
        var start = new ViewPosition(1, 5);
        var end = new ViewPosition(1, 10);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.IsEmpty;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Normalize_WhenStartLineBeforeEndLine_ReturnsSameRange()
    {
        // arrange
        var start = new ViewPosition(1, 5);
        var end = new ViewPosition(3, 10);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.Normalize();

        // assert
        Assert.AreEqual(start, result.Start);
        Assert.AreEqual(end, result.End);
    }

    [TestMethod]
    public void Normalize_WhenStartLineAfterEndLine_ReturnsSwappedRange()
    {
        // arrange
        var start = new ViewPosition(3, 10);
        var end = new ViewPosition(1, 5);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.Normalize();

        // assert
        Assert.AreEqual(end, result.Start);
        Assert.AreEqual(start, result.End);
    }

    [TestMethod]
    public void Normalize_WhenSameLineAndStartColumnBeforeEndColumn_ReturnsSameRange()
    {
        // arrange
        var start = new ViewPosition(1, 5);
        var end = new ViewPosition(1, 10);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.Normalize();

        // assert
        Assert.AreEqual(start, result.Start);
        Assert.AreEqual(end, result.End);
    }

    [TestMethod]
    public void Normalize_WhenSameLineAndStartColumnAfterEndColumn_ReturnsSwappedRange()
    {
        // arrange
        var start = new ViewPosition(1, 10);
        var end = new ViewPosition(1, 5);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.Normalize();

        // assert
        Assert.AreEqual(end, result.Start);
        Assert.AreEqual(start, result.End);
    }

    [TestMethod]
    public void Normalize_WhenSameLineAndSameColumn_ReturnsSameRange()
    {
        // arrange
        var position = new ViewPosition(1, 5);
        var range = new SelectionViewRange(position, position);

        // act
        var result = range.Normalize();

        // assert
        Assert.AreEqual(position, result.Start);
        Assert.AreEqual(position, result.End);
    }

    [TestMethod]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act
        var result = range1.Equals(range2);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithDifferentStart_ReturnsFalse()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(2, 5), new ViewPosition(3, 10));

        // act
        var result = range1.Equals(range2);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentEnd_ReturnsFalse()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(4, 10));

        // act
        var result = range1.Equals(range2);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithObject_SameValues_ReturnsTrue()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        object range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act
        var result = range1.Equals(range2);

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithNull_ReturnsFalse()
    {
        // arrange
        var range = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act
        var result = range.Equals(null);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // arrange
        var range = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        object other = "not a SelectionViewRange";

        // act
        var result = range.Equals(other);

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetHashCode_WithSameValues_ReturnsSameHashCode()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act & assert
        Assert.AreEqual(range1.GetHashCode(), range2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WithDifferentValues_ReturnsDifferentHashCode()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(2, 6), new ViewPosition(4, 11));

        // act & assert
        Assert.AreNotEqual(range1.GetHashCode(), range2.GetHashCode());
    }

    [TestMethod]
    public void EqualityOperator_WithSameValues_ReturnsTrue()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act
        var result = range1 == range2;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void EqualityOperator_WithDifferentValues_ReturnsFalse()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(2, 6), new ViewPosition(4, 11));

        // act
        var result = range1 == range2;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void InequalityOperator_WithDifferentValues_ReturnsTrue()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(2, 6), new ViewPosition(4, 11));

        // act
        var result = range1 != range2;

        // assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void InequalityOperator_WithSameValues_ReturnsFalse()
    {
        // arrange
        var range1 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));
        var range2 = new SelectionViewRange(new ViewPosition(1, 5), new ViewPosition(3, 10));

        // act
        var result = range1 != range2;

        // assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        // arrange
        var start = new ViewPosition(1, 5);
        var end = new ViewPosition(3, 10);
        var range = new SelectionViewRange(start, end);

        // act
        var result = range.ToString();

        // assert
        Assert.AreEqual("(1, 5) -> (3, 10)", result);
    }
}
