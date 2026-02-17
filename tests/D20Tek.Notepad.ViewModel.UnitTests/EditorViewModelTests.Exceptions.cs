namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelExceptionsTests
{
    [TestMethod]
    public void Constructor_WithNullSession_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>([ExcludeFromCodeCoverage] () => new EditorViewModel(null!));
    }
}
