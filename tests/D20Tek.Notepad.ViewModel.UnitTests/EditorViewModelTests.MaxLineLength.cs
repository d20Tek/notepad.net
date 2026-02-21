namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelMaxLineLengthTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void GetMaxLineLength_WithMultipleLines_ReturnsLongestLineLength()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Short", "This is a longer line", "Medium line"]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(21, result); // "This is a longer line" = 21 chars
    }

    [TestMethod]
    public void GetMaxLineLength_WithSingleLine_ReturnsThatLineLength()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(11, result);
    }

    [TestMethod]
    public void GetMaxLineLength_WithEmptyLines_ReturnsZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel([""]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void GetMaxLineLength_WithMixedEmptyAndNonEmptyLines_ReturnsLongestNonEmpty()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["", "Hello", "", "Hi", ""]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(5, result); // "Hello" = 5 chars
    }

    [TestMethod]
    public void GetMaxLineLength_WithAllSameLengthLines_ReturnsThatLength()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["abc", "def", "ghi"]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void GetMaxLineLength_WithVeryLongLine_ReturnsCorrectLength()
    {
        // arrange
        var longLine = new string('x', 1000);
        var (viewModel, _) = CreateViewModel([longLine, "short"]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(1000, result);
    }

    [TestMethod]
    public void GetMaxLineLength_WithUnicodeCharacters_CountsCorrectly()
    {
        // arrange - Unicode characters count as their string length
        var (viewModel, _) = CreateViewModel(["Hello 世界", "Short"]);

        // act
        var result = viewModel.GetMaxLineLength();

        // assert
        Assert.AreEqual(8, result); // "Hello 世界" = 8 chars (including space and 2 Chinese chars)
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(session, new EditorCommandService(session));
        return (viewModel, session);
    }
}
