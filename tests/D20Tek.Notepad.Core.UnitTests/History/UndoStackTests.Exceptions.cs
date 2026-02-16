namespace D20Tek.Notepad.Core.UnitTests.History;

[TestClass]
[ExcludeFromCodeCoverage]
public class UndoStackExceptionTests
{
    [TestMethod]
    public void Push_WithNullOperation_ThrowsArgumentNullException()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => stack.Push(null!));
    }

    [TestMethod]
    public void BeginGroup_WhenGroupAlreadyActive_ThrowsInvalidOperationException()
    {
        // arrange
        var stack = new UndoStack();
        stack.BeginGroup();

        // act & assert
        Assert.ThrowsExactly<InvalidOperationException>(() => stack.BeginGroup());
    }

    [TestMethod]
    public void EndGroup_WhenNoGroupActive_ThrowsInvalidOperationException()
    {
        // arrange
        var stack = new UndoStack();

        // act & assert
        Assert.ThrowsExactly<InvalidOperationException>(() => stack.EndGroup());
    }
}