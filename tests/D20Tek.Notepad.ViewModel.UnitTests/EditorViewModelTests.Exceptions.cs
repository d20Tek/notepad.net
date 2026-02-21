namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelExceptionsTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void Constructor_WithNullSession_ThrowsArgumentNullException()
    {
        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => new EditorViewModel(null!, null!));
    }

    [TestMethod]
    public void Constructor_WithNullCommandService_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession(["test"]);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => new EditorViewModel(session, null!));
    }

    [TestMethod]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // arrange
        var session = CreateSession(["test"]);
        var commands = new EditorCommandService(session);

        // act & assert
        Assert.ThrowsExactly<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => new EditorViewModel(session, commands, null!));
    }

    [TestMethod]
    public void Constructor_WithDefaultOverload_UsesDefaultSettings()
    {
        // arrange
        var session = CreateSession(["test"]);
        var commands = new EditorCommandService(session);

        // act
        var viewModel = new EditorViewModel(session, commands);

        // assert
        Assert.AreSame(EditorSettings.Default, viewModel.Settings);
    }

    [TestMethod]
    public void Constructor_WithCustomSettings_UsesProvidedSettings()
    {
        // arrange
        var session = CreateSession(["test"]);
        var commands = new EditorCommandService(session);
        var customSettings = new EditorSettings { MouseWheelScrollLines = 10 };

        // act
        var viewModel = new EditorViewModel(session, commands, customSettings);

        // assert
        Assert.AreSame(customSettings, viewModel.Settings);
        Assert.AreEqual(10, viewModel.Settings.MouseWheelScrollLines);
    }

    private static EditorSession CreateSession(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        return new EditorSession(doc);
    }
}
