namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelLineNumbersTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void IsLineNumbersEnabled_WithDefaultSettings_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act & assert
        Assert.IsFalse(viewModel.IsLineNumbersEnabled);
    }

    [TestMethod]
    public void IsLineNumbersEnabled_WithLineNumbersSettings_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithLineNumbers(["Hello World"]);

        // act & assert
        Assert.IsTrue(viewModel.IsLineNumbersEnabled);
    }

    [TestMethod]
    public void ToggleLineNumbers_WhenDisabled_EnablesLineNumbers()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        viewModel.ToggleLineNumbers();

        // assert
        Assert.IsTrue(viewModel.IsLineNumbersEnabled);
        Assert.IsTrue(viewModel.Settings.LineNumbersEnabled);
    }

    [TestMethod]
    public void ToggleLineNumbers_WhenEnabled_DisablesLineNumbers()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithLineNumbers(["Hello World"]);

        // act
        viewModel.ToggleLineNumbers();

        // assert
        Assert.IsFalse(viewModel.IsLineNumbersEnabled);
        Assert.IsFalse(viewModel.Settings.LineNumbersEnabled);
    }

    [TestMethod]
    public void ToggleLineNumbers_FiresLineNumbersChangedEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        bool? eventValue = null;
        viewModel.LineNumbersChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.ToggleLineNumbers();

        // assert
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void ToggleLineNumbers_WhenEnabled_FiresLineNumbersChangedEventWithFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithLineNumbers(["Hello World"]);
        bool? eventValue = null;
        viewModel.LineNumbersChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.ToggleLineNumbers();

        // assert
        Assert.IsFalse(eventValue);
    }

    [TestMethod]
    public void GutterWidth_WhenDisabled_ReturnsZero()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act & assert
        Assert.AreEqual(0, viewModel.GutterWidth);
    }

    [TestMethod]
    public void GutterWidth_WithSingleDigitLineCount_ReturnsThree()
    {
        // arrange
        var lines = Enumerable.Repeat("line", 5).ToArray();
        var (viewModel, _) = CreateViewModelWithLineNumbers(lines);

        // act & assert
        Assert.AreEqual(3, viewModel.GutterWidth);
    }

    [TestMethod]
    public void GutterWidth_WithMultiDigitLineCount_ReturnsFive()
    {
        // arrange
        var lines = Enumerable.Repeat("line", 100).ToArray();
        var (viewModel, _) = CreateViewModelWithLineNumbers(lines);

        // act & assert
        Assert.AreEqual(5, viewModel.GutterWidth);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModel(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            EditorSettings.Default,
            new MockClipboardService());
        return (viewModel, session);
    }

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithLineNumbers(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var settings = new EditorSettings { LineNumbersEnabled = true };
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            settings,
            new MockClipboardService());
        return (viewModel, session);
    }
}
