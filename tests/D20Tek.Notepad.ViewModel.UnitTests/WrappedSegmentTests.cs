namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class WrappedSegmentTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // arrange
        var startColumn = 10;
        var length = 5;
        var text = "Hello";

        // act
        var segment = new WrappedSegment(startColumn, length, text);

        // assert
        Assert.AreEqual(startColumn, segment.StartColumn);
        Assert.AreEqual(length, segment.Length);
        Assert.AreEqual(text, segment.Text);
    }

    [TestMethod]
    public void With_ChangingAllProperties_CreatesNewInstanceWithAllUpdates()
    {
        // arrange
        var original = new WrappedSegment(0, 5, "Hello");

        // act
        var modified = original with
        {
            StartColumn = 10,
            Length = 6,
            Text = "World!"
        };

        // assert
        Assert.AreNotSame(original, modified);
        Assert.AreEqual(10, modified.StartColumn);
        Assert.AreEqual(6, modified.Length);
        Assert.AreEqual("World!", modified.Text);
    }

    [TestMethod]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        var segment1 = new WrappedSegment(0, 5, "Hello");
        var segment2 = new WrappedSegment(0, 5, "Hello");

        // act
        var areEqual = segment1 == segment2;

        // assert
        Assert.IsTrue(areEqual);
        Assert.AreEqual(segment1, segment2);
    }

    [TestMethod]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // arrange
        var segment1 = new WrappedSegment(0, 5, "Hello");
        var segment2 = new WrappedSegment(10, 5, "Hello");

        // act
        var areNotEqual = segment1 != segment2;

        // assert
        Assert.IsTrue(areNotEqual);
        Assert.AreNotEqual(segment1, segment2);
    }
}
