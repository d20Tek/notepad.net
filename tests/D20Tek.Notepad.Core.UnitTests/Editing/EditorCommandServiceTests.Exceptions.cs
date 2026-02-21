namespace D20Tek.Notepad.Core.UnitTests.Editing;

[TestClass]
[ExcludeFromCodeCoverage]
public class EditorCommandServiceExceptionsTests
{
    [TestMethod]
    public void Constructor_WithNullSession_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(() => new EditorCommandService(null!));
    }
}