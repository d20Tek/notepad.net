namespace D20Tek.Notepad.ViewModel.UnitTests;

[TestClass]
public class EditorViewModelStatusBarTests
{
    private static readonly DocumentFactory _docFactory = new();

    [TestMethod]
    public void IsStatusBarEnabled_WithDefaultSettings_ReturnsFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act & assert
        Assert.IsFalse(viewModel.IsStatusBarEnabled);
    }

    [TestMethod]
    public void IsStatusBarEnabled_WithStatusBarSettings_ReturnsTrue()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithStatusBar(["Hello World"]);

        // act & assert
        Assert.IsTrue(viewModel.IsStatusBarEnabled);
    }

    [TestMethod]
    public void ToggleStatusBar_WhenDisabled_EnablesStatusBar()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);

        // act
        viewModel.ToggleStatusBar();

        // assert
        Assert.IsTrue(viewModel.IsStatusBarEnabled);
        Assert.IsTrue(viewModel.Settings.StatusBarEnabled);
    }

    [TestMethod]
    public void ToggleStatusBar_WhenEnabled_DisablesStatusBar()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithStatusBar(["Hello World"]);

        // act
        viewModel.ToggleStatusBar();

        // assert
        Assert.IsFalse(viewModel.IsStatusBarEnabled);
        Assert.IsFalse(viewModel.Settings.StatusBarEnabled);
    }

    [TestMethod]
    public void ToggleStatusBar_FiresStatusBarChangedEvent()
    {
        // arrange
        var (viewModel, _) = CreateViewModel(["Hello World"]);
        bool? eventValue = null;
        viewModel.StatusBarChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.ToggleStatusBar();

        // assert
        Assert.IsTrue(eventValue);
    }

    [TestMethod]
    public void ToggleStatusBar_WhenEnabled_FiresStatusBarChangedEventWithFalse()
    {
        // arrange
        var (viewModel, _) = CreateViewModelWithStatusBar(["Hello World"]);
        bool? eventValue = null;
        viewModel.StatusBarChanged += (enabled) => eventValue = enabled;

        // act
        viewModel.ToggleStatusBar();

        // assert
        Assert.IsFalse(eventValue);
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

    private static (EditorViewModel viewModel, EditorSession session) CreateViewModelWithStatusBar(string[] lines)
    {
        var textLines = lines.Select(l => new TextLine(l)).ToList();
        var doc = _docFactory.Create(new DocumentData(textLines, Encoding.UTF8, LineEndingStyle.CRLF));
        var session = new EditorSession(doc);
        var settings = new EditorSettings { StatusBarEnabled = true };
        var viewModel = new EditorViewModel(
            session,
            new EditorCommandService(session),
            settings,
            new MockClipboardService());
        return (viewModel, session);
    }
}
